
using Detailly.Domain.Common.Enums;

namespace Detailly.Application.Modules.Booking.Bookings.Commands.Cancel;

public sealed class CancelBookingCommandHandler(IAppDbContext context, IAppCurrentUser appCurrentUser)
    : IRequestHandler<CancelBookingCommand, Unit>
{
    public async Task<Unit> Handle(CancelBookingCommand request, CancellationToken ct)
    {
        var now = DateTime.UtcNow;

        if (appCurrentUser.ApplicationUserId is null)
            throw new DetaillyBusinessRuleException("AUTH_REQUIRED", "Authentication required.");

        var customerId = appCurrentUser.ApplicationUserId.Value;

        var booking = await context.Bookings
            .FirstOrDefaultAsync(b => b.Id == request.BookingId && !b.IsDeleted, ct);

        if (booking is null)
            throw new DetaillyNotFoundException("Booking not found.");

        if (booking.CustomerId != customerId)
            throw new DetaillyBusinessRuleException("BOOKING_FORBIDDEN", "You can only cancel your own booking.");

        if (booking.Status is BookingStatus.Cancelled or BookingStatus.Completed)
            return Unit.Value;

        booking.Status = BookingStatus.Cancelled;
        booking.ReservationExpiresAtUtc = null;
        booking.ModifiedAtUtc = now;

        await context.SaveChangesAsync(ct);
        return Unit.Value;
    }
}