
namespace Detailly.Application.Modules.Vehicle.Vehicles.Queries.GetById;

public class GetVehicleByIdQueryDto
{
    public required int Id { get; init; }
    public required string Brand { get; init; }
    public required string Model { get; init; }
    public required int YearOfManufacture { get; init; }
    public required string? LicencePlate { get; init; }
    public required string? Notes { get; init; }
    public required GetVehicleByIdQueryDtoVehicleCategory VehicleCategory { get; init; }
}

public class GetVehicleByIdQueryDtoVehicleCategory
{
    public required string Name { get; init; }
}

