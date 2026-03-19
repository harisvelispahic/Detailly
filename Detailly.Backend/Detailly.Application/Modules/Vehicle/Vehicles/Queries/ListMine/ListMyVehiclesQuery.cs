namespace Detailly.Application.Modules.Vehicle.Vehicles.Queries.ListMine;

public class ListMyVehiclesQuery : BasePagedQuery<ListMyVehiclesQueryDto>
{
    public string? Search { get; init; }
}

