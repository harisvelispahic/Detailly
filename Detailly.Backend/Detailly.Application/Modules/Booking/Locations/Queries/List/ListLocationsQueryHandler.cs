namespace Detailly.Application.Modules.Booking.Locations.Queries.List;

public sealed class ListLocationsQueryHandler(IAppDbContext context)
    : IRequestHandler<ListLocationsQuery, PageResult<ListLocationsQueryDto>>
{
    public async Task<PageResult<ListLocationsQueryDto>> Handle(ListLocationsQuery request, CancellationToken ct)
    {
        var q = context.Locations.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var s = request.Search.Trim();
            q = q.Where(x => x.Name.Contains(s));
        }

        if (request.LocationType is not null)
            q = q.Where(l => l.LocationType == request.LocationType.Value);

        var projectedQuery = q
            .OrderBy(l => l.Name)
            .Select(l => new ListLocationsQueryDto
            {
                Id = l.Id,
                Name = l.Name,
                LocationType = l.LocationType,
                TotalBays = l.TotalBays,
                City = l.Address.City,
                Country = l.Address.Country
            });

        return await PageResult<ListLocationsQueryDto>.FromQueryableAsync(projectedQuery, request.Paging, ct);
    }
}
