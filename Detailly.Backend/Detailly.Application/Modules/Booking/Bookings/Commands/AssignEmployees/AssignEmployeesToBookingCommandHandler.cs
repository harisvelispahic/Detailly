
using Detailly.Domain.Common.Enums;
using Detailly.Domain.Entities.Booking;

namespace Detailly.Application.Modules.Booking.Bookings.Commands.AssignEmployees;

public sealed class AssignEmployeesToBookingCommandHandler(IAppDbContext context, IAppCurrentUser appCurrentUser)
    : IRequestHandler<AssignEmployeesToBookingCommand, Unit>
{
    public async Task<Unit> Handle(AssignEmployeesToBookingCommand request, CancellationToken ct)
    {
        var now = DateTime.UtcNow;

        if (!appCurrentUser.IsAuthenticated || appCurrentUser.ApplicationUserId is null)
            throw new DetaillyBusinessRuleException("AUTH_REQUIRED", "Authentication required.");

        if (!appCurrentUser.IsAdmin && !appCurrentUser.IsManager)
            throw new DetaillyBusinessRuleException("FORBIDDEN", "Manager/Admin access required.");

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

        // Validate each employee has a shift covering this interval
        var shiftCoveringEmployees = await context.EmployeeShifts
            .Where(s =>
                !s.IsDeleted &&
                s.ShopLocationId == booking.ShopLocationId &&
                s.EmployeeWorkMode == shiftMode &&
                s.StartUtc <= booking.StartUtc &&
                s.EndUtc >= booking.EndUtc &&
                employeeIds.Contains(s.EmployeeId))
            .Select(s => s.EmployeeId)
            .Distinct()
            .ToListAsync(ct);

        if (shiftCoveringEmployees.Count != employeeIds.Count)
            throw new DetaillyBusinessRuleException("ASSIGN_NOT_ON_SHIFT",
                "One or more employees do not have a shift covering the booking time.");

        // Validate no overlaps with other assigned bookings
        var overlappingEmployeeIds = await context.BookingEmployeeAssignments
            .Where(a =>
                !a.IsDeleted &&
                employeeIds.Contains(a.EmployeeId) &&
                a.BookingId != booking.Id &&
                (a.Booking.Status == BookingStatus.Confirmed || a.Booking.Status == BookingStatus.Completed) &&
                a.Booking.StartUtc < booking.EndUtc &&
                a.Booking.EndUtc > booking.StartUtc)
            .Select(a => a.EmployeeId)
            .Distinct()
            .ToListAsync(ct);

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