
using Detailly.Domain.Common.Enums;
using Detailly.Domain.Entities.Booking;

namespace Detailly.Application.Modules.Booking.Bookings.Commands.CreateHold;

public sealed class CreateBookingHoldCommandHandler(IAppDbContext context, IAppCurrentUser appCurrentUser)
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

        await using var tx = await context.Database.BeginTransactionAsync(ct);

        var package = await context.ServicePackages
            .FirstOrDefaultAsync(x => x.Id == request.ServicePackageId && !x.IsDeleted, ct);

        if (package is null)
            throw new DetaillyNotFoundException("Service package not found.");

        var baseDuration = package.BaseDurationMinutes ?? 0;
        var baseEmployees = package.BaseRequiredEmployees ?? 1;

        var addonIds = (request.AddonItemIds ?? new List<int>())
            .Distinct()
            .ToList();

        // Prevent selecting add-ons already in base package
        if (addonIds.Count > 0)
        {
            var baseItemIds = await context.ServicePackageItemAssignments
                .Where(a => a.ServicePackageId == request.ServicePackageId && !a.IsDeleted)
                .Select(a => a.ServicePackageItemId)
                .ToListAsync(ct);

            if (addonIds.Intersect(baseItemIds).Any())
                throw new DetaillyBusinessRuleException("BOOKING_ADDON_DUPLICATE",
                    "One or more add-ons are already included in the selected package.");
        }

        var addons = new List<ServicePackageItemEntity>();
        if (addonIds.Count > 0)
        {
            addons = await context.ServicePackageItems
                .Where(i => addonIds.Contains(i.Id) && !i.IsDeleted && i.IsAddon && i.IsActive)
                .ToListAsync(ct);

            if (addons.Count != addonIds.Count)
                throw new DetaillyBusinessRuleException("BOOKING_ADDON_INVALID",
                    "One or more add-on items are invalid or inactive.");
        }

        var addonsDuration = addons.Sum(x => x.DurationMinutes);
        var addonsEmployeesMax = addons.Count == 0 ? 0 : addons.Max(x => x.RequiredEmployees);
        var addonsPrice = addons.Sum(x => x.Price);

        var totalDurationMinutes = baseDuration + addonsDuration;
        if (totalDurationMinutes <= 0)
            throw new DetaillyBusinessRuleException("BOOKING_DURATION_INVALID", "Package duration is invalid.");

        var requiredEmployees = Math.Max(baseEmployees, addonsEmployeesMax);
        var requiredBays = request.ServiceMode == ServiceMode.InShop ? 1 : 0;

        var endUtc = request.StartUtc.AddMinutes(totalDurationMinutes);
        var totalPrice = package.Price + addonsPrice;

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
            var totalBaysAtLocation = await context.Locations
                .Where(l => l.Id == request.ShopLocationId && !l.IsDeleted)
                .Select(l => l.TotalBays)
                .FirstOrDefaultAsync(ct);

            var usedBays = blockingBookings.Sum(b => b.RequiredBays);
            if (totalBaysAtLocation - usedBays < requiredBays)
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
        await context.SaveChangesAsync(ct); // get booking.Id

        // Add BookingItems snapshots (add-ons)
        if (addons.Count > 0)
        {
            var bookingItems = addons.Select(a => new BookingItemEntity
            {
                BookingId = booking.Id,
                ServicePackageItemId = a.Id,
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