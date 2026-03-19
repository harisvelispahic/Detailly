namespace Detailly.Application.Modules.Vehicle.Vehicles.Queries.List;

public class ListVehiclesQueryHandler(IAppDbContext ctx, IAppCurrentUser appCurrentUser)
    : IRequestHandler<ListVehiclesQuery, PageResult<ListVehiclesQueryDto>>
{
    public async Task<PageResult<ListVehiclesQueryDto>> Handle(ListVehiclesQuery request, CancellationToken ct)
    {
        // Controller may limit access, but enforce admin here too
        if (!appCurrentUser.IsAuthenticated || !appCurrentUser.IsAdmin)
            throw new DetaillyForbiddenException("Only admins can list vehicles.");

        var q = ctx.Vehicles.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var s = request.Search.Trim();
            q = q.Where(x => x.Model.Contains(s) || x.Brand.Contains(s));
        }

        var projectedQuery = q.OrderBy(x => x.Brand)
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

        return await PageResult<ListVehiclesQueryDto>.FromQueryableAsync(projectedQuery, request.Paging, ct);
    }
}

