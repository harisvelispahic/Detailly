namespace Detailly.Application.Modules.Vehicle.Vehicles.Queries.ListMine;

public class ListMyVehiclesQueryDto
{
    public required int Id { get; init; }
    public required string Brand { get; init; }
    public required string Model { get; init; }
    public required int YearOfManufacture { get; init; }
    public required string? LicencePlate { get; init; }
    public required string? Notes { get; init; }
    public required ListMyVehiclesQueryDtoVehicleCategory VehicleCategory { get; init; }
}

public class ListMyVehiclesQueryDtoVehicleCategory
{
    public required string Name { get; init; }
    public required decimal BasePriceMultiplier { get; init; }
}


