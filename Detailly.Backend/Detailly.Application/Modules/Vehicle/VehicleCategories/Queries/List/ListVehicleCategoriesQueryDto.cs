namespace Detailly.Application.Modules.Vehicle.VehicleCategories.Queries.List;

public class ListVehicleCategoriesQueryDto
{
    public required int Id { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }
    public decimal BasePriceMultiplier { get; init; }
}
