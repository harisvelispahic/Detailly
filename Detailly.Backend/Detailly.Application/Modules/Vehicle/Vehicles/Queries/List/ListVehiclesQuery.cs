using Detailly.Application.Modules.Catalog.ProductCategories.Queries.List;

namespace Detailly.Application.Modules.Vehicle.Vehicles.Queries.List
{
    public class ListVehiclesQuery : IRequest<List<ListVehiclesQueryDto>>
    {
        public string? Search { get; init; }
    }
}
