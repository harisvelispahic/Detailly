
namespace Detailly.Application.Modules.Vehicle.Vehicles.Commands.Delete;

public class DeleteVehicleCommand : IRequest<Unit>
{
    public required int Id { get; set; }
}
