namespace Detailly.Application.Modules.Booking.Bookings.Commands.AssignEmployees;

public sealed class AssignEmployeesToBookingCommand : IRequest<Unit>
{
    [JsonIgnore]
    public int BookingId { get; set; }

    // The employees you want assigned to this booking (full replacement list)
    public List<int> EmployeeIds { get; set; } = new();
}