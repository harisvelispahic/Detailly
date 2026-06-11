using Detailly.Domain.Entities.Booking;

namespace Detailly.Application.Modules.Booking.EmployeeShifts.Commands.Create;

public sealed class CreateEmployeeShiftCommandHandler(
    IAppDbContext context,
    IAppAuthorizationService authService,
    TimeProvider timeProvider)
    : IRequestHandler<CreateEmployeeShiftCommand, int>
{
    public async Task<int> Handle(CreateEmployeeShiftCommand request, CancellationToken ct)
    {
        authService.EnsureAdminOrManager();

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

        var localStart = TimeZoneInfo.ConvertTimeFromUtc(
            DateTime.SpecifyKind(request.StartUtc, DateTimeKind.Utc), LocationOpeningHoursEntity.LocalZone);
        var localEnd = TimeZoneInfo.ConvertTimeFromUtc(
            DateTime.SpecifyKind(request.EndUtc, DateTimeKind.Utc), LocationOpeningHoursEntity.LocalZone);

        var dayOfWeek = (int)localStart.DayOfWeek;
        var openingHours = await context.LocationOpeningHours
            .FirstOrDefaultAsync(h => h.ShopLocationId == request.ShopLocationId && h.DayOfWeek == dayOfWeek && !h.IsDeleted, ct);

        if (openingHours?.IsClosed == true)
            throw new DetaillyBusinessRuleException("SHIFT_LOCATION_CLOSED", "The location is closed on the selected day.");

        var openTime  = openingHours?.OpenTime  ?? LocationOpeningHoursEntity.DefaultOpenTime;
        var closeTime = openingHours?.CloseTime ?? LocationOpeningHoursEntity.DefaultCloseTime;

        if (localStart.TimeOfDay < openTime)
            throw new DetaillyBusinessRuleException("SHIFT_BEFORE_OPEN", $"Shift cannot start before the location opens at {openTime:hh\\:mm}.");

        if (localEnd.TimeOfDay > closeTime)
            throw new DetaillyBusinessRuleException("SHIFT_AFTER_CLOSE", $"Shift cannot end after the location closes at {closeTime:hh\\:mm}.");

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
            CreatedAtUtc = timeProvider.GetUtcNow().UtcDateTime
        };

        context.EmployeeShifts.Add(shift);
        await context.SaveChangesAsync(ct);

        return shift.Id;
    }
}