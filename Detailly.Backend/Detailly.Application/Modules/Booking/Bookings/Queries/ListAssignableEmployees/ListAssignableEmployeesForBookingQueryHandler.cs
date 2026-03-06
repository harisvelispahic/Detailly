using Detailly.Domain.Common.Enums;

namespace Detailly.Application.Modules.Booking.Bookings.Queries.ListAssignableEmployees;

public sealed class ListAssignableEmployeesForBookingQueryHandler(IAppDbContext context, IAppCurrentUser appCurrentUser)
    : IRequestHandler<ListAssignableEmployeesForBookingQuery, List<ListAssignableEmployeesForBookingQueryDto>>
{
    public async Task<List<ListAssignableEmployeesForBookingQueryDto>> Handle(
        ListAssignableEmployeesForBookingQuery request,
        CancellationToken ct)
    {
        if (!appCurrentUser.IsAuthenticated || appCurrentUser.ApplicationUserId is null)
            throw new DetaillyBusinessRuleException("AUTH_REQUIRED", "Authentication required.");

        if (!appCurrentUser.IsAdmin && !appCurrentUser.IsManager)
            throw new DetaillyBusinessRuleException("FORBIDDEN", "Manager/Admin access required.");

        var booking = await context.Bookings
            .AsNoTracking()
            .Include(b => b.EmployeeAssignments)
            .FirstOrDefaultAsync(b => b.Id == request.BookingId && !b.IsDeleted, ct);

        if (booking is null)
            throw new DetaillyNotFoundException("Booking not found.");

        // Only meaningful once booking is real (PendingPayment/Confirmed/etc.)
        if (booking.Status is BookingStatus.Cancelled or BookingStatus.Expired)
            return new List<ListAssignableEmployeesForBookingQueryDto>();

        var shiftMode = booking.ServiceMode == ServiceMode.InShop
            ? EmployeeWorkMode.InShop
            : EmployeeWorkMode.Mobile;

        var alreadyAssignedEmployeeIds = booking.EmployeeAssignments
            .Select(a => a.EmployeeId)
            .ToList();

        // Employees with shift fully covering the booking interval
        var shiftCoveringEmployees = await context.EmployeeShifts
            .AsNoTracking()
            .Where(s =>
                !s.IsDeleted &&
                s.ShopLocationId == booking.ShopLocationId &&
                s.EmployeeWorkMode == shiftMode &&
                s.StartUtc <= booking.StartUtc &&
                s.EndUtc >= booking.EndUtc)
            .Select(s => s.EmployeeId)
            .Distinct()
            .ToListAsync(ct);

        if (shiftCoveringEmployees.Count == 0)
            return new List<ListAssignableEmployeesForBookingQueryDto>();

        // Load employees (only employees, enabled if you have that rule)
        var employees = await context.ApplicationUsers
            .AsNoTracking()
            .Where(u =>
                shiftCoveringEmployees.Contains(u.Id) &&
                u.IsEmployee &&
                u.IsEnabled)
            .Select(u => new
            {
                u.Id,
                FullName = u.FirstName + " " + u.LastName
            })
            .ToListAsync(ct);

        // Find which employees are already assigned to other bookings overlapping this booking time
        // (Confirmed/Completed are the ones that matter operationally; you can include PendingPayment holds if you want)
        var overlappingAssignedEmployeeIds = await context.BookingEmployeeAssignments
            .AsNoTracking()
            .Where(a =>
                !a.IsDeleted &&
                a.BookingId != booking.Id &&
                a.Booking.ShopLocationId == booking.ShopLocationId &&
                a.Booking.ServiceMode == booking.ServiceMode &&
                (a.Booking.Status == BookingStatus.Confirmed || a.Booking.Status == BookingStatus.Completed) &&
                a.Booking.StartUtc < booking.EndUtc &&
                a.Booking.EndUtc > booking.StartUtc)
            .Select(a => a.EmployeeId)
            .Distinct()
            .ToListAsync(ct);

        var results = employees
            .Where(e => !alreadyAssignedEmployeeIds.Contains(e.Id))
            .OrderBy(e => e.FullName)
            .Select(e => new ListAssignableEmployeesForBookingQueryDto
            {
                EmployeeId = e.Id,
                FullName = e.FullName,
                HasOverlappingAssignment = overlappingAssignedEmployeeIds.Contains(e.Id)
            })
            .ToList();

        return results;
    }
}