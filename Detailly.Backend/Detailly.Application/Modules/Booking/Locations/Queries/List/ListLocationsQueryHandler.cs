
namespace Detailly.Application.Modules.Booking.Locations.Queries.List;

public sealed class ListLocationsQueryHandler(IAppDbContext context)
    : IRequestHandler<ListLocationsQuery, List<ListLocationsQueryDto>>
{
    public async Task<List<ListLocationsQueryDto>> Handle(ListLocationsQuery request, CancellationToken ct)
    {
        var q = context.Locations
            .AsNoTracking()
            .Where(l => !l.IsDeleted);

        if (request.LocationType is not null)
            q = q.Where(l => l.LocationType == request.LocationType.Value);

        return await q
            .OrderBy(l => l.Name)
            .Select(l => new ListLocationsQueryDto
            {
                Id = l.Id,
                Name = l.Name,
                LocationType = l.LocationType,
                TotalBays = l.TotalBays,
                City = l.Address.City,
                Country = l.Address.Country
            })
            .ToListAsync(ct);
    }
}