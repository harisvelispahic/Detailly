
namespace Detailly.Application.Modules.Vehicle.Vehicles.Commands.Update;

public class UpdateVehicleCommand : IRequest<Unit>
{
    public required int Id { get; set; }
    public string? Brand { get; set; }
    public string? Model { get; set; }
    public int? YearOfManufacture { get; set; }
    public string? LicencePlate { get; set; }
    public string? Notes { get; set; }
    public int? VehicleCategoryId { get; set; }
}
