
namespace Detailly.Application.Modules.Vehicle.Vehicles.Queries.List;

public class ListVehiclesQueryHandler(IAppDbContext ctx)
    : IRequestHandler<ListVehiclesQuery, List<ListVehiclesQueryDto>>
{
    public async Task<List<ListVehiclesQueryDto>> Handle(ListVehiclesQuery request, CancellationToken ct)
    {
        var q = ctx.Vehicles.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            q = q.Where(x => x.Model.Contains(request.Search) || x.Brand.Contains(request.Search));
        }

        var vehicles = q.OrderBy(x => x.Model)
            .Select(x => new ListVehiclesQueryDto
            {
                Id = x.Id,
                Model = x.Model,
                Brand = x.Brand,
                ApplicationUserId = x.ApplicationUserId,
                VehicleCategoryId = x.VehicleCategoryId,
                YearOfManufacture = x.YearOfManufacture
            });

        if (!vehicles.Any())
            throw new MarketNotFoundException("No vehicles found.");

        return await vehicles.ToListAsync(ct);
    }
}
