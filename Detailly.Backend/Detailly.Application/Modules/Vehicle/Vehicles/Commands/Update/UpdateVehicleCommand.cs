
namespace Detailly.Application.Modules.Vehicle.Vehicles.Commands.Update;

public class UpdateVehicleCommand : IRequest<Unit>
{
    public required int Id { get; set; }
    public string? Model { get; set; }
    public string? Brand { get; set; }
    public int? YearOfManufacture { get; set; }
    }

