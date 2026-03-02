
namespace Detailly.Application.Modules.Booking.EmployeeShifts.Commands.Update;

public sealed class UpdateEmployeeShiftCommandHandler(IAppDbContext context, IAppCurrentUser currentUser)
    : IRequestHandler<UpdateEmployeeShiftCommand, Unit>
{
    public async Task<Unit> Handle(UpdateEmployeeShiftCommand request, CancellationToken ct)
    {
        if (currentUser.ApplicationUserId is null)
            throw new DetaillyBusinessRuleException("AUTH_REQUIRED", "Authentication required.");

        if (!currentUser.IsAdmin && !currentUser.IsManager)
            throw new DetaillyBusinessRuleException("FORBIDDEN", "Only Admin/Manager can manage shifts.");

        if (request.EndUtc <= request.StartUtc)
            throw new DetaillyBusinessRuleException("SHIFT_INVALID", "Shift end must be after shift start.");

        var shift = await context.EmployeeShifts
            .FirstOrDefaultAsync(s => s.Id == request.Id && !s.IsDeleted, ct);

        if (shift is null)
            throw new DetaillyNotFoundException("Shift not found.");

        // Ensure location exists
        var locationExists = await context.Locations
            .AnyAsync(l => l.Id == request.ShopLocationId && !l.IsDeleted, ct);

        if (!locationExists)
            throw new DetaillyNotFoundException("Location not found.");

        // Ensure employee exists and is employee
        var employee = await context.ApplicationUsers
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == request.EmployeeId && !u.IsDeleted, ct);

        if (employee is null)
            throw new DetaillyNotFoundException("Employee not found.");

        if (!employee.IsEmployee)
            throw new DetaillyBusinessRuleException("SHIFT_NOT_EMPLOYEE", "Selected user is not an employee.");

        // Prevent overlaps (exclude current shift)
        var overlaps = await context.EmployeeShifts
            .AnyAsync(s =>
                !s.IsDeleted &&
                s.Id != request.Id &&
                s.EmployeeId == request.EmployeeId &&
                s.EmployeeWorkMode == request.EmployeeWorkMode &&
                s.StartUtc < request.EndUtc &&
                s.EndUtc > request.StartUtc, ct);

        if (overlaps)
            throw new DetaillyBusinessRuleException("SHIFT_OVERLAP", "Employee already has an overlapping shift.");

        shift.EmployeeId = request.EmployeeId;
        shift.ShopLocationId = request.ShopLocationId;
        shift.EmployeeWorkMode = request.EmployeeWorkMode;
        shift.StartUtc = request.StartUtc;
        shift.EndUtc = request.EndUtc;
        shift.ModifiedAtUtc = DateTime.UtcNow;

        await context.SaveChangesAsync(ct);
        return Unit.Value;
    }
}