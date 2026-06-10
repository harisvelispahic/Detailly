using Detailly.Domain.Common.Enums;
using Detailly.Domain.Entities.Booking;

namespace Detailly.Application.Modules.Booking.Bookings.Commands.AssignEmployees;

public sealed class AssignEmployeesToBookingCommandHandler(
    IAppDbContext context,
    IAppAuthorizationService authService,
    TimeProvider timeProvider)
    : IRequestHandler<AssignEmployeesToBookingCommand, Unit>
{
    public async Task<Unit> Handle(AssignEmployeesToBookingCommand request, CancellationToken ct)
    {
        var now = timeProvider.GetUtcNow().UtcDateTime;

        authService.EnsureAdminOrManager();

        var employeeIds = (request.EmployeeIds ?? new List<int>())
            .Distinct()
            .ToList();

        if (employeeIds.Count == 0)
            throw new DetaillyBusinessRuleException("ASSIGN_EMPTY", "At least one employee must be provided.");

        await using var tx = await context.Database.BeginTransactionAsync(ct);

        var booking = await context.Bookings
            .Include(b => b.EmployeeAssignments)
            .FirstOrDefaultAsync(b => b.Id == request.BookingId && !b.IsDeleted, ct);

        if (booking is null)
            throw new DetaillyNotFoundException("Booking not found.");

        if (booking.Status is BookingStatus.Cancelled or BookingStatus.Expired)
            throw new DetaillyBusinessRuleException("BOOKING_NOT_ASSIGNABLE", "Cannot assign employees to a cancelled/expired booking.");

        // If you want: only allow assignment for Confirmed bookings
        // if (booking.Status != BookingStatus.Confirmed)
        //     throw new DetaillyBusinessRuleException("BOOKING_NOT_CONFIRMED", "Assign employees only after booking is confirmed.");

        var travelMinutes = booking.TravelTimeMinutes ?? 0;
        var departureUtc  = booking.StartUtc.AddMinutes(-travelMinutes);
        var returnUtc     = booking.EndUtc.AddMinutes(travelMinutes);

        if (employeeIds.Count > booking.RequiredEmployees)
            throw new DetaillyBusinessRuleException("ASSIGN_TOO_MANY",
                $"Too many employees. Booking requires {booking.RequiredEmployees}.");

        // Validate employees exist and are employees
        var existingEmployees = await context.ApplicationUsers
            .Where(u => employeeIds.Contains(u.Id) && u.IsEmployee && u.IsEnabled)
            .Select(u => u.Id)
            .ToListAsync(ct);

        if (existingEmployees.Count != employeeIds.Count)
            throw new DetaillyBusinessRuleException("ASSIGN_EMPLOYEE_INVALID", "One or more employees are invalid or not enabled.");

        var shiftMode = booking.ServiceMode == ServiceMode.InShop
            ? EmployeeWorkMode.InShop
            : EmployeeWorkMode.Mobile;

        // Validate each employee has a shift covering the full effective [departure, return] window
        var shiftCoveringEmployees = await context.EmployeeShifts
            .Where(s =>
                !s.IsDeleted &&
                s.ShopLocationId == booking.ShopLocationId &&
                s.EmployeeWorkMode == shiftMode &&
                s.StartUtc <= departureUtc &&
                s.EndUtc >= returnUtc &&
                employeeIds.Contains(s.EmployeeId))
            .Select(s => s.EmployeeId)
            .Distinct()
            .ToListAsync(ct);

        if (shiftCoveringEmployees.Count != employeeIds.Count)
            throw new DetaillyBusinessRuleException("ASSIGN_NOT_ON_SHIFT",
                "One or more employees do not have a shift covering the booking time.");

        // Validate no overlaps with other assigned bookings.
        // SQL pre-filter + in-memory precision to account for other bookings' travel time.
        var candidateConflicts = await context.BookingEmployeeAssignments
            .Where(a =>
                !a.IsDeleted &&
                employeeIds.Contains(a.EmployeeId) &&
                a.BookingId != booking.Id &&
                (a.Booking.Status == BookingStatus.Confirmed || a.Booking.Status == BookingStatus.Completed) &&
                a.Booking.StartUtc < returnUtc &&
                a.Booking.EndUtc > departureUtc)
            .Select(a => new { a.EmployeeId, a.Booking.StartUtc, a.Booking.EndUtc, a.Booking.TravelTimeMinutes })
            .ToListAsync(ct);

        var overlappingEmployeeIds = candidateConflicts
            .Where(a =>
            {
                var bDep = a.StartUtc.AddMinutes(-(a.TravelTimeMinutes ?? 0));
                var bRet = a.EndUtc.AddMinutes(a.TravelTimeMinutes ?? 0);
                return bDep < returnUtc && bRet > departureUtc;
            })
            .Select(a => a.EmployeeId)
            .Distinct()
            .ToList();

        if (overlappingEmployeeIds.Count > 0)
            throw new DetaillyBusinessRuleException("ASSIGN_OVERLAP",
                "One or more employees are already assigned to an overlapping booking.");

        // Replace strategy:
        // - remove existing assignments
        // - add new assignments
        // This makes UI simple (send the final list).
        if (booking.EmployeeAssignments.Count > 0)
        {
            // NOTE: if EmployeeAssignments is IReadOnlyCollection, you might not be able to .Clear().
            // So we remove via DbSet.
            context.BookingEmployeeAssignments.RemoveRange(booking.EmployeeAssignments);
        }

        var newAssignments = employeeIds.Select(id => new BookingEmployeeAssignmentEntity
        {
            BookingId = booking.Id,
            EmployeeId = id,
            AssignedAtUtc = now,
            CreatedAtUtc = now
        });

        context.BookingEmployeeAssignments.AddRange(newAssignments);

        await context.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);

        return Unit.Value;
    }
}