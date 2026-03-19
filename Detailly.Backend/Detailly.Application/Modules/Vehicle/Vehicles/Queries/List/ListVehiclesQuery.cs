namespace Detailly.Application.Modules.Vehicle.Vehicles.Queries.List;

public class ListVehiclesQuery : BasePagedQuery<ListVehiclesQueryDto>
{
    public string? Search { get; init; }
}

