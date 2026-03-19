namespace Detailly.Application.Modules.Vehicle.Vehicles.Commands.Delete;

public class DeleteVehicleCommand : IRequest<Unit>
{
    [JsonIgnore]
    public int Id { get; set; }
}
