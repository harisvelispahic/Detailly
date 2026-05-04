using Detailly.Application.Common;

namespace Detailly.Application.Modules.Vehicle.VehicleCategories.Queries.List;

public class ListVehicleCategoriesQueryHandler(IAppDbContext context)
    : IRequestHandler<ListVehicleCategoriesQuery, PageResult<ListVehicleCategoriesQueryDto>>
{
    public async Task<PageResult<ListVehicleCategoriesQueryDto>> Handle(
        ListVehicleCategoriesQuery request,
        CancellationToken ct)
    {
        var q = context.VehicleCategories.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim();
            q = q.Where(c => c.Name.Contains(search) ||
                             (c.Description != null && c.Description.Contains(search)));
        }

        var projected = q
            .OrderBy(c => c.Name)
            .Select(c => new ListVehicleCategoriesQueryDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                BasePriceMultiplier = c.BasePriceMultiplier
            });

        return await PageResult<ListVehicleCategoriesQueryDto>.FromQueryableAsync(projected, request.Paging, ct);
    }
}
