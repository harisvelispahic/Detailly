
namespace Detailly.Application.Modules.Booking.Bookings.Queries.GetById;

public sealed class GetBookingByIdQueryHandler(IAppDbContext context, IAppCurrentUser appCurrentUser)
    : IRequestHandler<GetBookingByIdQuery, GetBookingByIdQueryDto>
{
    public async Task<GetBookingByIdQueryDto> Handle(GetBookingByIdQuery request, CancellationToken ct)
    {
        if (appCurrentUser.ApplicationUserId is null)
            throw new DetaillyBusinessRuleException("AUTH_REQUIRED", "Authentication required.");

        var customerId = appCurrentUser.ApplicationUserId.Value;

        var booking = await context.Bookings
            .AsNoTracking()
            .Where(b => b.Id == request.Id && !b.IsDeleted)
            .Select(b => new
            {
                Booking = b,
                PackageName = b.ServicePackage.Name,
                PaymentTransactionId = b.PaymentTransaction != null ? (int?)b.PaymentTransaction.Id : null,
                PaymentStatus = b.PaymentTransaction != null ? (Detailly.Domain.Common.Enums.PaymentTransactionStatus?)b.PaymentTransaction.Status : null
            })
            .FirstOrDefaultAsync(ct);

        if (booking is null)
            throw new DetaillyNotFoundException("Booking not found.");

        if (booking.Booking.CustomerId != customerId)
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

        var b = booking.Booking;

        return new GetBookingByIdQueryDto
        {
            Id = b.Id,
            Status = b.Status,
            ServiceMode = b.ServiceMode,
            StartUtc = b.StartUtc,
            EndUtc = b.EndUtc,
            TotalPrice = b.TotalPrice,
            RequiredEmployees = b.RequiredEmployees,
            RequiredBays = b.RequiredBays,
            ReservationExpiresAtUtc = b.ReservationExpiresAtUtc,
            Notes = b.Notes,
            ServicePackageId = b.ServicePackageId,
            ServicePackageName = booking.PackageName,
            Addons = addons,
            VehicleIds = vehicleIds,
            PaymentTransactionId = booking.PaymentTransactionId,
            PaymentStatus = booking.PaymentStatus
        };
    }
}