namespace Detailly.Application.Modules.Booking.EmployeeShifts.Commands.Delete;

public sealed class DeleteEmployeeShiftCommandHandler(IAppDbContext context, IAppAuthorizationService authService)
    : IRequestHandler<DeleteEmployeeShiftCommand, Unit>
{
    public async Task<Unit> Handle(DeleteEmployeeShiftCommand request, CancellationToken ct)
    {
        authService.EnsureAdminOrManager();

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