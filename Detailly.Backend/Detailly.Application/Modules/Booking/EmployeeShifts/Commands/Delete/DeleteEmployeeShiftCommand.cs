
namespace Detailly.Application.Modules.Booking.EmployeeShifts.Commands.Delete;

public sealed class DeleteEmployeeShiftCommand : IRequest<Unit>
{
    public required int Id { get; set; }
}