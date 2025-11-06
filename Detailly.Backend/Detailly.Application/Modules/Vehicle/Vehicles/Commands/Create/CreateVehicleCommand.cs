
namespace Detailly.Application.Modules.Vehicle.Vehicles.Commands.Create;

public class CreateVehicleCommand : IRequest<int>
{
    public required string Brand { get; set; }
    public required string Model { get; set; }
    public required int YearOfManufacture { get; set; }
    public required int VehicleCategoryId { get; set; }
    public required string LicencePlate { get; set; }
    
    public string? Notes { get; set; } = string.Empty;
}
