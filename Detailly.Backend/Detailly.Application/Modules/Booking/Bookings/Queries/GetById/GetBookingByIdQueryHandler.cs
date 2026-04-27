using Detailly.Domain.Common.Enums;

namespace Detailly.Application.Modules.Booking.Bookings.Queries.GetById;

public sealed class GetBookingByIdQueryHandler(IAppDbContext context, IAppCurrentUser appCurrentUser)
    : IRequestHandler<GetBookingByIdQuery, GetBookingByIdQueryDto>
{
    public async Task<GetBookingByIdQueryDto> Handle(GetBookingByIdQuery request, CancellationToken ct)
    {
        if (appCurrentUser.ApplicationUserId is null)
            throw new DetaillyBusinessRuleException("AUTH_REQUIRED", "Authentication required.");

        var customerId = appCurrentUser.ApplicationUserId.Value;

        var b = await context.Bookings
            .AsNoTracking()
            .Where(x => x.Id == request.Id && !x.IsDeleted)
            .Select(x => new
            {
                x.Id,
                x.Status,
                x.ServiceMode,
                x.StartUtc,
                x.EndUtc,
                x.TotalPrice,
                x.MobileSurchargeFee,
                x.RequiredEmployees,
                x.RequiredBays,
                x.TravelTimeMinutes,
                x.ReservationExpiresAtUtc,
                x.Notes,
                x.ServicePackageId,
                PackageName = x.ServicePackage.Name,
                x.CustomerId
            })
            .FirstOrDefaultAsync(ct);

        if (b is null)
            throw new DetaillyNotFoundException("Booking not found.");

        if (b.CustomerId != customerId)
            throw new DetaillyBusinessRuleException("BOOKING_FORBIDDEN", "You can only view your own booking.");

        var addons = await context.BookingItems
            .AsNoTracking()
            .Where(x => x.BookingId == request.Id && !x.IsDeleted && x.IsAddon)
            .Select(x => new BookingAddonDto
            {
                BookingItemId = x.ServicePackageItemId,
                Name = x.ServicePackageItem.Name,
                PriceSnapshot = x.PriceSnapshot,
                DurationMinutesSnapshot = x.DurationMinutesSnapshot,
                RequiredEmployeesSnapshot = x.RequiredEmployeesSnapshot
            })
            .ToListAsync(ct);

        var vehicleIds = await context.BookingVehicleAssignments
            .AsNoTracking()
            .Where(x => x.BookingId == request.Id && !x.IsDeleted)
            .Select(x => x.VehicleId)
            .ToListAsync(ct);

        // ✅ latest PAYMENT attempt (not refunds)
        var latestPaymentAttempt = await context.PaymentTransactions
            .AsNoTracking()
            .Where(x =>
                !x.IsDeleted &&
                x.BookingId == request.Id &&
                x.TransactionType == TransactionType.Payment)
            .OrderByDescending(x => x.TransactionDate)
            .ThenByDescending(x => x.Id)
            .Select(x => new { x.Id, x.Status })
            .FirstOrDefaultAsync(ct);

        return new GetBookingByIdQueryDto
        {
            Id = b.Id,
            Status = b.Status,
            ServiceMode = b.ServiceMode,
            StartUtc = b.StartUtc,
            EndUtc = b.EndUtc,
            TotalPrice = b.TotalPrice,
            MobileSurchargeFee = b.MobileSurchargeFee,
            RequiredEmployees = b.RequiredEmployees,
            RequiredBays = b.RequiredBays,
            TravelTimeMinutes = b.TravelTimeMinutes,
            DepartureUtc = b.TravelTimeMinutes > 0 ? b.StartUtc.AddMinutes(-b.TravelTimeMinutes.Value) : null,
            ReturnUtc    = b.TravelTimeMinutes > 0 ? b.EndUtc.AddMinutes(b.TravelTimeMinutes.Value) : null,
            ReservationExpiresAtUtc = b.ReservationExpiresAtUtc,
            Notes = b.Notes,
            ServicePackageId = b.ServicePackageId,
            ServicePackageName = b.PackageName,
            Addons = addons,
            VehicleIds = vehicleIds,
            PaymentTransactionId = latestPaymentAttempt?.Id,
            PaymentStatus = latestPaymentAttempt is null ? null : (PaymentTransactionStatus?)latestPaymentAttempt.Status
        };
    }
}