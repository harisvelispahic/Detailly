using Detailly.Application.Abstractions.Booking;
using Detailly.Application.Modules.Booking.Locations.Commands.Create;
using Detailly.Domain.Entities.Booking;

namespace Detailly.Application.Modules.Booking.Locations.Commands.Update;

public sealed class UpdateLocationCommandHandler(
    IAppDbContext context,
    IAppCurrentUser appCurrentUser,
    IRoadDistanceService roadDistanceService)
    : IRequestHandler<UpdateLocationCommand, Unit>
{
    public async Task<Unit> Handle(UpdateLocationCommand request, CancellationToken ct)
    {
        var now = DateTime.UtcNow;

        if (!appCurrentUser.IsAuthenticated)
            throw new DetaillyBusinessRuleException("AUTH_REQUIRED", "Authentication required.");

        if (!appCurrentUser.IsAdmin && !appCurrentUser.IsManager)
            throw new DetaillyBusinessRuleException("FORBIDDEN", "Only admin/manager can manage locations.");

        var location = await context.Locations
            .Include(l => l.Address)
            .FirstOrDefaultAsync(l => l.Id == request.Id && !l.IsDeleted, ct);

        if (location is null)
            throw new DetaillyNotFoundException("Location not found.");

        if (!string.IsNullOrWhiteSpace(request.Name))
            location.Name = request.Name.Trim();

        if (request.Description is not null)
            location.Description = request.Description.Trim();

        if (request.TotalBays is not null)
        {
            if (request.TotalBays.Value <= 0)
                throw new DetaillyBusinessRuleException("LOCATION_BAYS_REQUIRED", "TotalBays must be > 0.");
            location.TotalBays = request.TotalBays.Value;
        }

        // Update address fields when provided
        if (request.Address is not null)
        {
            var a = request.Address;
            bool addressChanged = false;

            // Only flag as changed when the trimmed incoming value differs from what is stored,
            // so ORS (road-distance geocoding) is not called on a no-op update.
            if (!string.IsNullOrWhiteSpace(a.Street)    && a.Street.Trim()     != location.Address.Street)     { location.Address.Street     = a.Street.Trim();     addressChanged = true; }
            if (!string.IsNullOrWhiteSpace(a.City)      && a.City.Trim()       != location.Address.City)       { location.Address.City       = a.City.Trim();       addressChanged = true; }
            if (!string.IsNullOrWhiteSpace(a.PostalCode)&& a.PostalCode.Trim() != location.Address.PostalCode) { location.Address.PostalCode = a.PostalCode.Trim(); addressChanged = true; }
            if (a.Region is not null                    && a.Region.Trim()     != location.Address.Region)     { location.Address.Region     = a.Region.Trim(); }
            if (!string.IsNullOrWhiteSpace(a.Country)   && a.Country.Trim()    != location.Address.Country)    { location.Address.Country    = a.Country.Trim();    addressChanged = true; }

            if (addressChanged)
            {
                var coords = await roadDistanceService.GetCoordinatesAsync(
                    location.Address.Street ?? string.Empty,
                    location.Address.City ?? string.Empty,
                    location.Address.PostalCode,
                    location.Address.Country ?? string.Empty,
                    ct);

                location.Address.Latitude  = coords?.Latitude;
                location.Address.Longitude = coords?.Longitude;
                location.Address.ModifiedAtUtc = now;
            }
        }

        // Replace opening hours when provided
        if (request.OpeningHours?.Count > 0)
        {
            // Must use IgnoreQueryFilters: the unique index on (ShopLocationId, DayOfWeek)
            // applies to soft-deleted rows too, so delete+insert would violate it.
            // Instead we update rows in-place and restore any that were soft-deleted.
            var allExisting = await context.LocationOpeningHours
                .IgnoreQueryFilters()
                .Where(h => h.ShopLocationId == location.Id)
                .ToListAsync(ct);

            if (OpeningHoursHaveChanged(request.OpeningHours, allExisting))
            {
                foreach (var h in request.OpeningHours)
                {
                    var openTime  = h.IsClosed || h.OpenHour  is null ? (TimeSpan?)null : new TimeSpan(h.OpenHour.Value,  h.OpenMinute  ?? 0, 0);
                    var closeTime = h.IsClosed || h.CloseHour is null ? (TimeSpan?)null : new TimeSpan(h.CloseHour.Value, h.CloseMinute ?? 0, 0);

                    var row = allExisting.FirstOrDefault(e => e.DayOfWeek == h.DayOfWeek);
                    if (row is not null)
                    {
                        row.IsDeleted     = false;
                        row.IsClosed      = h.IsClosed;
                        row.OpenTimeUtc   = openTime;
                        row.CloseTimeUtc  = closeTime;
                        row.ModifiedAtUtc = now;
                    }
                    else
                    {
                        context.LocationOpeningHours.Add(new LocationOpeningHoursEntity
                        {
                            ShopLocationId = location.Id,
                            DayOfWeek      = h.DayOfWeek,
                            IsClosed       = h.IsClosed,
                            OpenTimeUtc    = openTime,
                            CloseTimeUtc   = closeTime,
                            CreatedAtUtc   = now,
                        });
                    }
                }

                // Soft-delete any days that were not included in the incoming set
                var incomingDays = request.OpeningHours.Select(h => h.DayOfWeek).ToHashSet();
                foreach (var row in allExisting.Where(e => !e.IsDeleted && !incomingDays.Contains(e.DayOfWeek)))
                {
                    row.IsDeleted     = true;
                    row.ModifiedAtUtc = now;
                }
            }
        }

        location.ModifiedAtUtc = now;

        await context.SaveChangesAsync(ct);
        return Unit.Value;
    }

    private static bool OpeningHoursHaveChanged(List<LocationOpeningHoursInputDto> incoming, List<LocationOpeningHoursEntity> existing)
    {
        foreach (var h in incoming)
        {
            var openTime  = h.IsClosed || h.OpenHour  is null ? (TimeSpan?)null : new TimeSpan(h.OpenHour.Value,  h.OpenMinute  ?? 0, 0);
            var closeTime = h.IsClosed || h.CloseHour is null ? (TimeSpan?)null : new TimeSpan(h.CloseHour.Value, h.CloseMinute ?? 0, 0);

            var row = existing.FirstOrDefault(e => e.DayOfWeek == h.DayOfWeek);
            if (row is null || row.IsDeleted)    return true;
            if (row.IsClosed    != h.IsClosed)   return true;
            if (row.OpenTimeUtc  != openTime)    return true;
            if (row.CloseTimeUtc != closeTime)   return true;
        }

        var incomingDays = incoming.Select(h => h.DayOfWeek).ToHashSet();
        return existing.Any(e => !e.IsDeleted && !incomingDays.Contains(e.DayOfWeek));
    }
}
