
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

        var vehicles = q.OrderBy(x => x.Brand)
            .Select(x => new ListVehiclesQueryDto
            {
                Id = x.Id,
                Brand = x.Brand,
                Model = x.Model,
                YearOfManufacture = x.YearOfManufacture,
                LicencePlate = x.LicencePlate,
                Notes = x.Notes,
                VehicleCategory = new ListVehiclesQueryDtoVehicleCategory
                {
                    Name = x.VehicleCategory.Name
                },
            });

        //if (!vehicles.Any())
        //    throw new DetaillyNotFoundException("No vehicles found.");

        return await vehicles.ToListAsync(ct);
    }
}
