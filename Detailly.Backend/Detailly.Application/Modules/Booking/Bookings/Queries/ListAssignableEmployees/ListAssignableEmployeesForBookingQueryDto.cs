
namespace Detailly.Application.Modules.Booking.Bookings.Queries.ListAssignableEmployees;
public sealed class ListAssignableEmployeesForBookingQueryDto
{
    public int EmployeeId { get; set; }
    public string FullName { get; set; } = string.Empty;

    // Helpful UX: show if the employee is already assigned to another booking that overlaps the time
    public bool HasOverlappingAssignment { get; set; }
}