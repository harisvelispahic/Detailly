
using Detailly.Domain.Common.Enums;

namespace Detailly.Application.Modules.Booking.Bookings.Commands.Complete;

public sealed class CompleteBookingCommandHandler(IAppDbContext context, IAppCurrentUser appCurrentUser)
    : IRequestHandler<CompleteBookingCommand, Unit>
{
    public async Task<Unit> Handle(CompleteBookingCommand request, CancellationToken ct)
    {
        var now = DateTime.UtcNow;

        if (appCurrentUser.ApplicationUserId is null || !appCurrentUser.IsAuthenticated)
            throw new DetaillyBusinessRuleException("AUTH_REQUIRED", "Authentication required.");

        // Staff-only: Employee OR Manager OR Admin
        if (!appCurrentUser.IsEmployee && !appCurrentUser.IsManager && !appCurrentUser.IsAdmin)
            throw new DetaillyBusinessRuleException("FORBIDDEN", "Only staff can complete bookings.");

        var booking = await context.Bookings
            .FirstOrDefaultAsync(b => b.Id == request.BookingId && !b.IsDeleted, ct);

        if (booking is null)
            throw new DetaillyNotFoundException("Booking not found.");

        // idempotent
        if (booking.Status == BookingStatus.Completed)
            return Unit.Value;

        if (booking.Status != BookingStatus.Confirmed)
            throw new DetaillyBusinessRuleException(
                "BOOKING_NOT_COMPLETABLE",
                "Only confirmed bookings can be marked as completed.");

        booking.Status = BookingStatus.Completed;
        booking.ModifiedAtUtc = now;

        await context.SaveChangesAsync(ct);

        return Unit.Value;
    }
}