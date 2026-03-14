namespace Detailly.Application.Modules.Booking.EmployeeShifts.Commands.Delete;

public sealed class DeleteEmployeeShiftCommandHandler(IAppDbContext context, IAppCurrentUser currentUser)
    : IRequestHandler<DeleteEmployeeShiftCommand, Unit>
{
    public async Task<Unit> Handle(DeleteEmployeeShiftCommand request, CancellationToken ct)
    {
        if (currentUser.ApplicationUserId is null)
            throw new DetaillyBusinessRuleException("AUTH_REQUIRED", "Authentication required.");

        if (!currentUser.IsAdmin && !currentUser.IsManager)
            throw new DetaillyBusinessRuleException("FORBIDDEN", "Only Admin/Manager can manage shifts.");

        var shift = await context.EmployeeShifts
            .FirstOrDefaultAsync(s => s.Id == request.Id && !s.IsDeleted, ct);

        if (shift is null)
            return Unit.Value; // idempotent

        shift.IsDeleted = true;
        shift.ModifiedAtUtc = DateTime.UtcNow;

        await context.SaveChangesAsync(ct);
        return Unit.Value;
    }
}