namespace Detailly.Application.Modules.Vehicle.VehicleCategories.Queries.List;

public class ListVehicleCategoriesQueryHandler(IAppDbContext context)
    : IRequestHandler<ListVehicleCategoriesQuery, IList<ListVehicleCategoriesQueryDto>>
{
    public async Task<IList<ListVehicleCategoriesQueryDto>> Handle(
        ListVehicleCategoriesQuery request,
        CancellationToken ct)
    {
        return await context.VehicleCategories
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .Select(c => new ListVehicleCategoriesQueryDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                BasePriceMultiplier = c.BasePriceMultiplier
            })
            .ToListAsync(ct);
    }
}
