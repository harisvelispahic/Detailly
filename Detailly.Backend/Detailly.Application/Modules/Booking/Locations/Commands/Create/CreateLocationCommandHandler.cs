using Detailly.Application.Abstractions.Booking;
using Detailly.Domain.Entities.Booking;
using Detailly.Domain.Entities.Shared;

namespace Detailly.Application.Modules.Booking.Locations.Commands.Create;

public sealed class CreateLocationCommandHandler(
    IAppDbContext context,
    IAppAuthorizationService authService,
    IRoadDistanceService roadDistanceService)
    : IRequestHandler<CreateLocationCommand, int>
{
    public async Task<int> Handle(CreateLocationCommand request, CancellationToken ct)
    {
        var now = DateTime.UtcNow;

        authService.EnsureAdminOrManager();

        if (request.TotalBays <= 0)
            throw new DetaillyBusinessRuleException("LOCATION_BAYS_REQUIRED", "Shop locations must have TotalBays > 0.");

        var a = request.Address;

        // Geocode the address to obtain coordinates
        var coords = await roadDistanceService.GetCoordinatesAsync(
            a.Street, a.City, a.PostalCode, a.Country, ct);

        var address = new AddressEntity
        {
            Street     = a.Street.Trim(),
            City       = a.City.Trim(),
            PostalCode = a.PostalCode.Trim(),
            Region     = a.Region?.Trim(),
            Country    = a.Country.Trim(),
            Latitude   = coords?.Latitude,
            Longitude  = coords?.Longitude,
            CreatedAtUtc = now,
            ApplicationUserId = null // business/shared address, not user-owned
        };

        context.Addresses.Add(address);
        await context.SaveChangesAsync(ct);

        var location = new LocationEntity
        {
            Name        = request.Name.Trim(),
            Description = request.Description?.Trim(),
            TotalBays   = request.TotalBays,
            AddressId   = address.Id,
            CreatedAtUtc = now
        };

        context.Locations.Add(location);
        await context.SaveChangesAsync(ct);

        if (request.OpeningHours?.Count > 0)
        {
            foreach (var h in request.OpeningHours)
            {
                context.LocationOpeningHours.Add(new LocationOpeningHoursEntity
                {
                    ShopLocationId = location.Id,
                    DayOfWeek      = h.DayOfWeek,
                    IsClosed       = h.IsClosed,
                    OpenTime    = h.IsClosed ? null : (h.OpenHour is not null ? new TimeSpan(h.OpenHour.Value, h.OpenMinute ?? 0, 0) : null),
                    CloseTime   = h.IsClosed ? null : (h.CloseHour is not null ? new TimeSpan(h.CloseHour.Value, h.CloseMinute ?? 0, 0) : null),
                    CreatedAtUtc   = now
                });
            }

            await context.SaveChangesAsync(ct);
        }

        return location.Id;
    }
}
