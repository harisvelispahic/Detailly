namespace Detailly.Application.Modules.Booking.Locations.Queries.GetOpeningHours;

public sealed class GetLocationOpeningHoursQueryHandler(IAppDbContext context, IAppCurrentUser currentUser)
    : IRequestHandler<GetLocationOpeningHoursQuery, List<GetLocationOpeningHoursQueryDto>>
{
    public async Task<List<GetLocationOpeningHoursQueryDto>> Handle(
        GetLocationOpeningHoursQuery request, CancellationToken ct)
    {
        if (!currentUser.IsAuthenticated)
            throw new DetaillyForbiddenException("Authentication required.");

        // Materialize first to avoid EF-Core translation issues with TimeSpan formatting
        var rows = await context.LocationOpeningHours
            .AsNoTracking()
            .Where(h => h.ShopLocationId == request.LocationId && !h.IsDeleted)
            .OrderBy(h => h.DayOfWeek)
            .Select(h => new
            {
                h.DayOfWeek,
                h.IsClosed,
                h.OpenTimeUtc,
                h.CloseTimeUtc,
            })
            .ToListAsync(ct);

        return rows.Select(h => new GetLocationOpeningHoursQueryDto
        {
            DayOfWeek   = h.DayOfWeek,
            IsClosed    = h.IsClosed,
            OpenHour    = h.OpenTimeUtc?.Hours,
            OpenMinute  = h.OpenTimeUtc?.Minutes,
            CloseHour   = h.CloseTimeUtc?.Hours,
            CloseMinute = h.CloseTimeUtc?.Minutes,
        }).ToList();
    }
}
