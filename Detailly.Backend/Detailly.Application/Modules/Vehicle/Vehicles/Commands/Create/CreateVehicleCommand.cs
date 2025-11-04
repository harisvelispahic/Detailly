
namespace Detailly.Application.Modules.Vehicle.Vehicles.Commands.Create;

public class CreateVehicleCommand : IRequest<int>
{
    public required string Model { get; set; }
    public required string Brand { get; set; }
    public int YearOfManufacture { get; set; }

    public required int ApplicationUserId { get; set; }
    public required int VehicleCategoryId { get; set; }

}
