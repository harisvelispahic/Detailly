
using Detailly.Application.Abstractions.Booking;
using Detailly.Domain.Common.Enums;
using Detailly.Domain.Entities.Booking;

namespace Detailly.Application.Modules.Booking.Bookings.Commands.CreateHold;

public sealed class CreateBookingHoldCommandHandler(
    IAppDbContext context,
    IAppCurrentUser appCurrentUser,
    IBookingQuoteService quoteService)
    : IRequestHandler<CreateBookingHoldCommand, int>
{
    private static readonly TimeSpan HoldDuration = TimeSpan.FromMinutes(10);

    public async Task<int> Handle(CreateBookingHoldCommand request, CancellationToken ct)
    {
        var now = DateTime.UtcNow;

        if (appCurrentUser.ApplicationUserId is null)
            throw new DetaillyBusinessRuleException("AUTH_REQUIRED", "Authentication required.");

        var customerId = appCurrentUser.ApplicationUserId.Value;

        // 30-min boundary rule
        if (request.StartUtc.Second != 0 || request.StartUtc.Minute % 30 != 0)
            throw new DetaillyBusinessRuleException("BOOKING_TIME_INVALID",
                "Start time must be aligned to 30-minute intervals.");

        if (request.StartUtc <= now)
            throw new DetaillyBusinessRuleException("BOOKING_TIME_PAST",
                "Start time must be in the future.");

        if (request.ServiceMode == ServiceMode.Mobile && request.ServiceAddressId is null)
            throw new DetaillyBusinessRuleException("BOOKING_ADDRESS_REQUIRED",
                "Service address is required for mobile bookings.");

        // Quote = single source of truth for duration/employees/bays/price + validated addon set
        var quote = await quoteService.CalculateAsync(
            request.ServicePackageId,
            request.AddonItemIds,
            request.ServiceMode,
            request.VehicleIds,
            customerId,
            appCurrentUser.IsFleet,
            ct);

        var endUtc = request.StartUtc.AddMinutes(quote.TotalDurationMinutes);
        var totalPrice = quote.TotalPrice;
        var requiredEmployees = quote.RequiredEmployees;
        var requiredBays = quote.RequiredBays;

        await using var tx = await context.Database.BeginTransactionAsync(ct);

        // Ensure location exists (and read total bays once if needed)
        var location = await context.Locations
            .AsNoTracking()
            .FirstOrDefaultAsync(l => l.Id == request.ShopLocationId && !l.IsDeleted, ct);

        if (location is null)
            throw new DetaillyNotFoundException("Location not found.");

        // --- Capacity check (atomic) ---
        var shiftMode = request.ServiceMode == ServiceMode.InShop
            ? EmployeeWorkMode.InShop
            : EmployeeWorkMode.Mobile;

        var availableEmployees = await context.EmployeeShifts
            .Where(s =>
                !s.IsDeleted &&
                s.ShopLocationId == request.ShopLocationId &&
                s.EmployeeWorkMode == shiftMode &&
                s.StartUtc <= request.StartUtc &&
                s.EndUtc >= endUtc)
            .Select(s => s.EmployeeId)
            .Distinct()
            .CountAsync(ct);

        var blockingBookings = await context.Bookings
            .Where(b =>
                !b.IsDeleted &&
                b.ShopLocationId == request.ShopLocationId &&
                b.ServiceMode == request.ServiceMode &&
                (
                    b.Status == BookingStatus.Confirmed ||
                    (b.Status == BookingStatus.PendingPayment &&
                     b.ReservationExpiresAtUtc != null &&
                     b.ReservationExpiresAtUtc > now)
                ) &&
                b.StartUtc < endUtc &&
                b.EndUtc > request.StartUtc
            )
            .Select(b => new { b.RequiredEmployees, b.RequiredBays })
            .ToListAsync(ct);

        var usedEmployees = blockingBookings.Sum(b => b.RequiredEmployees);
        if (availableEmployees - usedEmployees < requiredEmployees)
            throw new DetaillyBusinessRuleException("BOOKING_NO_CAPACITY",
                "Not enough employees available for the selected time.");

        if (request.ServiceMode == ServiceMode.InShop)
        {
            var usedBays = blockingBookings.Sum(b => b.RequiredBays);
            if (location.TotalBays - usedBays < requiredBays)
                throw new DetaillyBusinessRuleException("BOOKING_NO_BAYS",
                    "No bays available for the selected time.");
        }

        var booking = new BookingEntity
        {
            TotalPrice = totalPrice,
            StartUtc = request.StartUtc,
            EndUtc = endUtc,
            RequiredEmployees = requiredEmployees,
            RequiredBays = requiredBays,
            ReservationExpiresAtUtc = now.Add(HoldDuration),
            Status = BookingStatus.PendingPayment,
            Notes = request.Notes?.Trim(),
            ServiceMode = request.ServiceMode,

            CustomerId = customerId,
            ShopLocationId = request.ShopLocationId,
            ServiceAddressId = request.ServiceAddressId,
            ServicePackageId = request.ServicePackageId,

            CreatedAtUtc = now
        };

        context.Bookings.Add(booking);
        await context.SaveChangesAsync(ct); // booking.Id

        // Add BookingItems snapshots (add-ons) from the quote result
        if (quote.Addons.Count > 0)
        {
            var bookingItems = quote.Addons.Select(a => new BookingItemEntity
            {
                BookingId = booking.Id,
                ServicePackageItemId = a.ServicePackageItemId,
                PriceSnapshot = a.Price,
                DurationMinutesSnapshot = a.DurationMinutes,
                RequiredEmployeesSnapshot = a.RequiredEmployees,
                IsAddon = true,
                CreatedAtUtc = now
            });

            context.BookingItems.AddRange(bookingItems);
        }

        // Vehicle assignments (optional)
        var vehicleIds = (request.VehicleIds ?? new List<int>())
            .Distinct()
            .ToList();

        if (vehicleIds.Count > 0)
        {
            var owned = await context.Vehicles
                .Where(v =>
                    vehicleIds.Contains(v.Id) &&
                    !v.IsDeleted &&
                    v.ApplicationUserId == customerId)
                .Select(v => v.Id)
                .ToListAsync(ct);

            if (owned.Count != vehicleIds.Count)
                throw new DetaillyBusinessRuleException("BOOKING_VEHICLE_INVALID",
                    "One or more vehicles are invalid.");

            var assignments = vehicleIds.Select(vid => new BookingVehicleAssignmentEntity
            {
                BookingId = booking.Id,
                VehicleId = vid,
                CreatedAtUtc = now
            });

            context.BookingVehicleAssignments.AddRange(assignments);
        }

        await context.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);

        return booking.Id;
    }
}