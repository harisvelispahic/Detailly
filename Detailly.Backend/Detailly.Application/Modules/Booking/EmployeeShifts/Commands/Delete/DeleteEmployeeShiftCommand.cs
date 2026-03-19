namespace Detailly.Application.Modules.Booking.EmployeeShifts.Commands.Delete;

public sealed class DeleteEmployeeShiftCommand : IRequest<Unit>
{
    [JsonIgnore]
    public int Id { get; set; }
}