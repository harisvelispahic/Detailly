using Detailly.Domain.Common.Enums;

namespace Detailly.Application.Modules.Booking.Bookings.Commands.Complete;

public sealed class CompleteBookingCommandHandler(
    IAppDbContext context,
    IAppAuthorizationService authService,
    IAppCurrentUser appCurrentUser,
    TimeProvider timeProvider)
    : IRequestHandler<CompleteBookingCommand, Unit>
{
    public async Task<Unit> Handle(CompleteBookingCommand request, CancellationToken ct)
    {
        var now = timeProvider.GetUtcNow().UtcDateTime;

        authService.EnsureAnyStaff();

        var booking = await context.Bookings
            .Include(b => b.EmployeeAssignments)
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

        if (now < booking.EndUtc)
            throw new DetaillyBusinessRuleException(
                "BOOKING_NOT_ENDED",
                "A booking cannot be completed before its scheduled end time.");

        // 🔒 Assignment rule:
        // - Admin/Manager can complete anything
        // - Employee can complete ONLY if assigned to that booking
        if (!appCurrentUser.IsAdmin && !appCurrentUser.IsManager)
        {
            var employeeId = appCurrentUser.ApplicationUserId.Value;

            // If nobody is assigned -> employee cannot complete (forces manager assignment first)
            if (booking.EmployeeAssignments is null || booking.EmployeeAssignments.Count == 0)
                throw new DetaillyBusinessRuleException(
                    "BOOKING_NOT_ASSIGNED",
                    "This booking has no assigned employees yet. A manager must assign staff before completion.");

            var isAssigned = booking.EmployeeAssignments.Any(a => a.EmployeeId == employeeId);

            if (!isAssigned)
                throw new DetaillyBusinessRuleException(
                    "BOOKING_FORBIDDEN",
                    "You can only complete bookings you are assigned to.");
        }

        booking.Status = BookingStatus.Completed;
        booking.ModifiedAtUtc = now;

        await context.SaveChangesAsync(ct);

        return Unit.Value;
    }
}