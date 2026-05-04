namespace Detailly.Application.Modules.Vehicle.VehicleCategories.Commands.Create;

public class CreateVehicleCategoryCommand : IRequest<int>
{
    public required string Name { get; init; }
    public string? Description { get; init; }
    public decimal BasePriceMultiplier { get; init; }
}
