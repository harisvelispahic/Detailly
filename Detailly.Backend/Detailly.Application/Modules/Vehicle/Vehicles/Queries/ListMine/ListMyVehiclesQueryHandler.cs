namespace Detailly.Application.Modules.Vehicle.Vehicles.Queries.ListMine;

public class ListMyVehiclesQueryHandler(IAppDbContext ctx, IAppCurrentUser appCurrentUser)
    : IRequestHandler<ListMyVehiclesQuery, PageResult<ListMyVehiclesQueryDto>>
{
    public async Task<PageResult<ListMyVehiclesQueryDto>> Handle(ListMyVehiclesQuery request, CancellationToken ct)
    {
        if (!appCurrentUser.IsAuthenticated || appCurrentUser.ApplicationUserId is null)
            throw new DetaillyUnauthorizedException("User is not authenticated.");

        var userId = appCurrentUser.ApplicationUserId.Value;

        var q = ctx.Vehicles.AsNoTracking()
            .Where(x => x.ApplicationUserId == userId);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var s = request.Search.Trim();
            q = q.Where(x => x.Model.Contains(s) || x.Brand.Contains(s));
        }

        var projectedQuery = q.OrderByDescending(x => x.CreatedAtUtc)
            .Select(x => new ListMyVehiclesQueryDto
            {
                Id = x.Id,
                Brand = x.Brand,
                Model = x.Model,
                YearOfManufacture = x.YearOfManufacture,
                LicencePlate = x.LicencePlate,
                Notes = x.Notes,
                VehicleCategory = new ListMyVehiclesQueryDtoVehicleCategory
                {
                    Name = x.VehicleCategory.Name
                },
            });

        return await PageResult<ListMyVehiclesQueryDto>.FromQueryableAsync(projectedQuery, request.Paging, ct);
    }
}
