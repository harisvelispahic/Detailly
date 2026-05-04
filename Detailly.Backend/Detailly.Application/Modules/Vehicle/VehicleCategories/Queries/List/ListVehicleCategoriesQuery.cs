using Detailly.Application.Common;

namespace Detailly.Application.Modules.Vehicle.VehicleCategories.Queries.List;

public class ListVehicleCategoriesQuery : BasePagedQuery<ListVehicleCategoriesQueryDto>
{
    public string? Search { get; init; }
}
