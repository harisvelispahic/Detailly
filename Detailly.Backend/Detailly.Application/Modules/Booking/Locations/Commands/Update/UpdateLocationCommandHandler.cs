using Detailly.Application.Abstractions.Booking;
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

            if (!string.IsNullOrWhiteSpace(a.Street))       { location.Address.Street     = a.Street.Trim();     addressChanged = true; }
            if (!string.IsNullOrWhiteSpace(a.City))         { location.Address.City       = a.City.Trim();       addressChanged = true; }
            if (!string.IsNullOrWhiteSpace(a.PostalCode))   { location.Address.PostalCode = a.PostalCode.Trim(); addressChanged = true; }
            if (a.Region is not null)                       { location.Address.Region     = a.Region.Trim();     }
            if (!string.IsNullOrWhiteSpace(a.Country))      { location.Address.Country    = a.Country.Trim();    addressChanged = true; }

            // Re-geocode when any address field changed
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
            }

            location.Address.ModifiedAtUtc = now;
        }

        // Replace opening hours when provided
        if (request.OpeningHours?.Count > 0)
        {
            var existing = await context.LocationOpeningHours
                .Where(h => h.ShopLocationId == location.Id && !h.IsDeleted)
                .ToListAsync(ct);

            foreach (var e in existing)
                e.IsDeleted = true;

            foreach (var h in request.OpeningHours)
            {
                context.LocationOpeningHours.Add(new LocationOpeningHoursEntity
                {
                    ShopLocationId = location.Id,
                    DayOfWeek      = h.DayOfWeek,
                    IsClosed       = h.IsClosed,
                    OpenTimeUtc    = h.IsClosed ? null : (h.OpenHour is not null ? new TimeSpan(h.OpenHour.Value, h.OpenMinute ?? 0, 0) : null),
                    CloseTimeUtc   = h.IsClosed ? null : (h.CloseHour is not null ? new TimeSpan(h.CloseHour.Value, h.CloseMinute ?? 0, 0) : null),
                    CreatedAtUtc   = now
                });
            }
        }

        location.ModifiedAtUtc = now;

        await context.SaveChangesAsync(ct);
        return Unit.Value;
    }
}
