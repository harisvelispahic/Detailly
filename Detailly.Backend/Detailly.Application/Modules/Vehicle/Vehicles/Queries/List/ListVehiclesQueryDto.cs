
namespace Detailly.Application.Modules.Vehicle.Vehicles.Queries.List;

public class ListVehiclesQueryDto
{
    public required int Id { get; init; }
    public required string Brand { get; init; }
    public required string Model { get; init; }
    public required int YearOfManufacture { get; init; }
    public required string? LicencePlate { get; init; }
    public required string? Notes { get; init; }
    public required ListVehiclesQueryDtoVehicleCategory VehicleCategory { get; init; }
}

public class ListVehiclesQueryDtoVehicleCategory
{
    public required string Name { get; init; }
}


