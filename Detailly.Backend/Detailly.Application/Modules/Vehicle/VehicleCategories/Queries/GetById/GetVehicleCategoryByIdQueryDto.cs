namespace Detailly.Application.Modules.Vehicle.VehicleCategories.Queries.GetById;

public class GetVehicleCategoryByIdQueryDto
{
    public required int Id { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }
    public decimal BasePriceMultiplier { get; init; }
}
