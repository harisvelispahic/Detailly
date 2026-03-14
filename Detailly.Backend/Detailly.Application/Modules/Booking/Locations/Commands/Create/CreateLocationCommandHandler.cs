using Detailly.Domain.Common.Enums;
using Detailly.Domain.Entities.Booking;
using Detailly.Domain.Entities.Shared;

namespace Detailly.Application.Modules.Booking.Locations.Commands.Create;

public sealed class CreateLocationCommandHandler(IAppDbContext context, IAppCurrentUser appCurrentUser)
    : IRequestHandler<CreateLocationCommand, int>
{
    public async Task<int> Handle(CreateLocationCommand request, CancellationToken ct)
    {
        var now = DateTime.UtcNow;

        if (!appCurrentUser.IsAuthenticated)
            throw new DetaillyBusinessRuleException("AUTH_REQUIRED", "Authentication required.");

        // Staff-only (you can tighten later)
        if (!appCurrentUser.IsAdmin && !appCurrentUser.IsManager)
            throw new DetaillyBusinessRuleException("FORBIDDEN", "Only admin/manager can manage locations.");

        // Must provide exactly one: AddressId OR Address payload
        var hasAddressId = request.AddressId is not null;
        var hasAddressPayload = request.Address is not null;

        if (hasAddressId == hasAddressPayload) // both true or both false
            throw new DetaillyBusinessRuleException(
                "LOCATION_ADDRESS_INVALID",
                "Provide either AddressId OR Address payload.");

        int addressId;

        if (hasAddressId)
        {
            var exists = await context.Addresses
                .AnyAsync(a => a.Id == request.AddressId!.Value && !a.IsDeleted, ct);

            if (!exists)
                throw new DetaillyNotFoundException("Address not found.");

            addressId = request.AddressId!.Value;
        }
        else
        {
            var a = request.Address!;

            // minimal sanity: at least city/country for address locations
            if (string.IsNullOrWhiteSpace(a.City) && string.IsNullOrWhiteSpace(a.Street))
                throw new DetaillyBusinessRuleException("ADDRESS_INVALID", "Address must contain at least City or Street.");

            var address = new AddressEntity
            {
                Street = a.Street?.Trim(),
                City = a.City?.Trim(),
                PostalCode = a.PostalCode?.Trim(),
                Region = a.Region?.Trim(),
                Country = a.Country?.Trim(),
                Latitude = a.Latitude,
                Longitude = a.Longitude,
                CreatedAtUtc = now
            };

            context.Addresses.Add(address);
            await context.SaveChangesAsync(ct);

            addressId = address.Id;
        }

        // Validate bays
        int totalBays;

        if (request.LocationType == LocationType.Shop)
        {
            totalBays = request.TotalBays ?? 0;
            if (totalBays <= 0)
                throw new DetaillyBusinessRuleException("LOCATION_BAYS_REQUIRED", "Shop locations must have TotalBays > 0.");
        }
        else
        {
            totalBays = 0; // address-type location has no bays
        }

        var location = new LocationEntity
        {
            Name = request.Name.Trim(),
            Description = request.Description?.Trim(),
            LocationType = request.LocationType,
            TotalBays = totalBays,
            AddressId = addressId,
            CreatedAtUtc = now
        };

        context.Locations.Add(location);
        await context.SaveChangesAsync(ct);

        return location.Id;
    }
}