using Detailly.Domain.Entities.Booking;

namespace Detailly.Application.Modules.Booking.EmployeeShifts.Commands.Create;

public sealed class CreateEmployeeShiftCommandHandler(IAppDbContext context, IAppCurrentUser currentUser)
    : IRequestHandler<CreateEmployeeShiftCommand, int>
{
    public async Task<int> Handle(CreateEmployeeShiftCommand request, CancellationToken ct)
    {
        if (currentUser.ApplicationUserId is null)
            throw new DetaillyBusinessRuleException("AUTH_REQUIRED", "Authentication required.");

        if (!currentUser.IsAdmin && !currentUser.IsManager)
            throw new DetaillyBusinessRuleException("FORBIDDEN", "Only Admin/Manager can manage shifts.");

        if (request.EndUtc <= request.StartUtc)
            throw new DetaillyBusinessRuleException("SHIFT_INVALID", "Shift end must be after shift start.");

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

        var dayOfWeek = (int)request.StartUtc.DayOfWeek;
        var openingHours = await context.LocationOpeningHours
            .FirstOrDefaultAsync(h => h.ShopLocationId == request.ShopLocationId && h.DayOfWeek == dayOfWeek && !h.IsDeleted, ct);

        if (openingHours?.IsClosed == true)
            throw new DetaillyBusinessRuleException("SHIFT_LOCATION_CLOSED", "The location is closed on the selected day.");

        var windowStart = request.StartUtc.Date.Add(openingHours?.OpenTimeUtc  ?? LocationOpeningHoursEntity.DefaultOpenTime);
        var windowEnd   = request.StartUtc.Date.Add(openingHours?.CloseTimeUtc ?? LocationOpeningHoursEntity.DefaultCloseTime);

        if (request.StartUtc < windowStart)
            throw new DetaillyBusinessRuleException("SHIFT_BEFORE_OPEN", $"Shift cannot start before the location opens at {windowStart:HH:mm} UTC.");

        if (request.EndUtc > windowEnd)
            throw new DetaillyBusinessRuleException("SHIFT_AFTER_CLOSE", $"Shift cannot end after the location closes at {windowEnd:HH:mm} UTC.");

        // Prevent overlaps for the same employee regardless of work mode —
        // an employee cannot physically be in two places at once.
        var overlaps = await context.EmployeeShifts
            .AnyAsync(s =>
                !s.IsDeleted &&
                s.EmployeeId == request.EmployeeId &&
                s.StartUtc < request.EndUtc &&
                s.EndUtc > request.StartUtc, ct);

        if (overlaps)
            throw new DetaillyBusinessRuleException("SHIFT_OVERLAP", "Employee already has an overlapping shift.");

        var shift = new EmployeeShiftEntity
        {
            EmployeeId = request.EmployeeId,
            ShopLocationId = request.ShopLocationId,
            EmployeeWorkMode = request.EmployeeWorkMode,
            StartUtc = request.StartUtc,
            EndUtc = request.EndUtc,
            CreatedAtUtc = DateTime.UtcNow
        };

        context.EmployeeShifts.Add(shift);
        await context.SaveChangesAsync(ct);

        return shift.Id;
    }
}