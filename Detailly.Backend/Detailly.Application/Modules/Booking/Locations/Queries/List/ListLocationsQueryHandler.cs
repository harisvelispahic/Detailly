namespace Detailly.Application.Modules.Booking.Locations.Queries.List;

public sealed class ListLocationsQueryHandler(IAppDbContext context)
    : IRequestHandler<ListLocationsQuery, PageResult<ListLocationsQueryDto>>
{
    public async Task<PageResult<ListLocationsQueryDto>> Handle(ListLocationsQuery request, CancellationToken ct)
    {
        var todayDow = (int)DateTime.UtcNow.DayOfWeek; // 0=Sunday…6=Saturday

        var q = context.Locations.AsNoTracking().Where(l => !l.IsDeleted);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var s = request.Search.Trim();
            q = q.Where(x => x.Name.Contains(s));
        }

        var projectedQuery = q
            .OrderBy(l => l.Name)
            .Select(l => new ListLocationsQueryDto
            {
                Id        = l.Id,
                Name      = l.Name,
                TotalBays = l.TotalBays,
                Street    = l.Address.Street,
                City      = l.Address.City,
                PostalCode = l.Address.PostalCode,
                Region    = l.Address.Region,
                Country   = l.Address.Country,
                IsOpenToday = l.LocationOpeningHours
                    .Any(h => !h.IsDeleted && h.DayOfWeek == todayDow && !h.IsClosed),
                IsTemporarilyClosed = l.IsTemporarilyClosed
            });

        return await PageResult<ListLocationsQueryDto>.FromQueryableAsync(projectedQuery, request.Paging, ct);
    }
}
