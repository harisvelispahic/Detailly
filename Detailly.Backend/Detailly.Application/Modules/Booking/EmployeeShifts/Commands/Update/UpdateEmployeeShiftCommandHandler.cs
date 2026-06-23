using Detailly.Domain.Entities.Booking;

namespace Detailly.Application.Modules.Booking.EmployeeShifts.Commands.Update;

public sealed class UpdateEmployeeShiftCommandHandler(
    IAppDbContext context,
    IAppAuthorizationService authService,
    TimeProvider timeProvider)
    : IRequestHandler<UpdateEmployeeShiftCommand, Unit>
{
    public async Task<Unit> Handle(UpdateEmployeeShiftCommand request, CancellationToken ct)
    {
        authService.EnsureAdminOrManager();

        var shift = await context.EmployeeShifts
            .FirstOrDefaultAsync(s => s.Id == request.Id && !s.IsDeleted, ct);

        if (shift is null)
            throw new DetaillyNotFoundException("Shift not found.");

        // Compose candidate values (existing + provided)
        var newEmployeeId = request.EmployeeId ?? shift.EmployeeId;
        var newShopLocationId = request.ShopLocationId ?? shift.ShopLocationId;
        var newStart = request.StartUtc ?? shift.StartUtc;
        var newEnd = request.EndUtc ?? shift.EndUtc;

        // Validate time ordering using composed values
        if (newEnd <= newStart)
            throw new DetaillyBusinessRuleException("SHIFT_INVALID", "Shift end must be after shift start.");

        // If location is changed, ensure it exists
        if (request.ShopLocationId != null)
        {
            var locationExists = await context.Locations
                .AnyAsync(l => l.Id == newShopLocationId && !l.IsDeleted, ct);

            if (!locationExists)
                throw new DetaillyNotFoundException("Location not found.");
        }

        // If employee is changed, ensure employee exists and is an employee
        if (request.EmployeeId != null)
        {
            var employee = await context.ApplicationUsers
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == newEmployeeId && !u.IsDeleted, ct);

            if (employee is null)
                throw new DetaillyNotFoundException("Employee not found.");

            if (!employee.IsEmployee)
                throw new DetaillyBusinessRuleException("SHIFT_NOT_EMPLOYEE", "Selected user is not an employee.");
        }

        var localStart = TimeZoneInfo.ConvertTimeFromUtc(
            DateTime.SpecifyKind(newStart, DateTimeKind.Utc), LocationOpeningHoursEntity.LocalZone);
        var localEnd = TimeZoneInfo.ConvertTimeFromUtc(
            DateTime.SpecifyKind(newEnd, DateTimeKind.Utc), LocationOpeningHoursEntity.LocalZone);

        var dayOfWeek = (int)localStart.DayOfWeek;
        var openingHours = await context.LocationOpeningHours
            .FirstOrDefaultAsync(h => h.ShopLocationId == newShopLocationId && h.DayOfWeek == dayOfWeek && !h.IsDeleted, ct);

        if (openingHours?.IsClosed == true)
            throw new DetaillyBusinessRuleException("SHIFT_LOCATION_CLOSED", "The location is closed on the selected day.");

        var openTime  = openingHours?.OpenTime  ?? LocationOpeningHoursEntity.DefaultOpenTime;
        var closeTime = openingHours?.CloseTime ?? LocationOpeningHoursEntity.DefaultCloseTime;

        if (localStart.TimeOfDay < openTime)
            throw new DetaillyBusinessRuleException("SHIFT_BEFORE_OPEN", $"Shift cannot start before the location opens at {openTime:hh\\:mm}.");

        if (localEnd.TimeOfDay > closeTime)
            throw new DetaillyBusinessRuleException("SHIFT_AFTER_CLOSE", $"Shift cannot end after the location closes at {closeTime:hh\\:mm}.");

        // If any of the properties that affect overlap are changed, check overlaps.
        var needOverlapCheck = request.EmployeeId != null
                               || request.EmployeeWorkMode != null
                               || request.StartUtc != null
                               || request.EndUtc != null;

        if (needOverlapCheck)
        {
            // Overlaps are checked regardless of work mode —
            // an employee cannot physically be in two places at once.
            var overlaps = await context.EmployeeShifts
                .AnyAsync(s =>
                    !s.IsDeleted &&
                    s.Id != request.Id &&
                    s.EmployeeId == newEmployeeId &&
                    s.StartUtc < newEnd &&
                    s.EndUtc > newStart, ct);

            if (overlaps)
                throw new DetaillyBusinessRuleException("SHIFT_OVERLAP", "Employee already has an overlapping shift.");
        }

        // Apply selective updates
        if (request.EmployeeId != null)
            shift.EmployeeId = request.EmployeeId.Value;

        if (request.ShopLocationId != null)
            shift.ShopLocationId = request.ShopLocationId.Value;

        if (request.EmployeeWorkMode != null)
            shift.EmployeeWorkMode = request.EmployeeWorkMode.Value;

        if (request.StartUtc != null)
            shift.StartUtc = request.StartUtc.Value;

        if (request.EndUtc != null)
            shift.EndUtc = request.EndUtc.Value;

        shift.ModifiedAtUtc = timeProvider.GetUtcNow().UtcDateTime;

        await context.SaveChangesAsync(ct);
        return Unit.Value;
    }
}