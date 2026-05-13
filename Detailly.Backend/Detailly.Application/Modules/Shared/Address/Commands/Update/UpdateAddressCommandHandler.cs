using Detailly.Application.Abstractions.Booking;
using Microsoft.Extensions.Logging;

namespace Detailly.Application.Modules.Shared.Address.Commands.Update;

public sealed class UpdateAddressCommandHandler(
    IAppDbContext context,
    IAppCurrentUser appCurrentUser,
    IRoadDistanceService roadDistanceService,
    ILogger<UpdateAddressCommandHandler> logger)
    : IRequestHandler<UpdateAddressCommand, Unit>
{
    public async Task<Unit> Handle(UpdateAddressCommand request, CancellationToken ct)
    {
        if (!appCurrentUser.IsAuthenticated || appCurrentUser.ApplicationUserId is null)
            throw new DetaillyUnauthorizedException("User is not authenticated.");

        var userId  = appCurrentUser.ApplicationUserId.Value;
        var isStaff = appCurrentUser.IsAdmin || appCurrentUser.IsManager;

        var address = await context.Addresses
            .FirstOrDefaultAsync(a =>
                a.Id == request.Id &&
                (isStaff || a.ApplicationUserId == userId),
                ct);

        if (address is null)
            throw new DetaillyNotFoundException($"Address with Id {request.Id} not found.");

        var locationChanged = false;

        if (request.Street is not null && request.Street.Trim() != address.Street)
        {
            address.Street = request.Street.Trim();
            locationChanged = true;
        }

        if (request.City is not null && request.City.Trim() != address.City)
        {
            address.City = request.City.Trim();
            locationChanged = true;
        }

        if (request.PostalCode is not null && request.PostalCode.Trim() != address.PostalCode)
        {
            address.PostalCode = request.PostalCode.Trim();
            locationChanged = true;
        }

        if (request.Region is not null)
            address.Region = string.IsNullOrWhiteSpace(request.Region) ? null : request.Region.Trim();

        if (request.Country is not null && request.Country.Trim() != address.Country)
        {
            address.Country = request.Country.Trim();
            locationChanged = true;
        }

        if (locationChanged)
        {
            var coords = await roadDistanceService.GetCoordinatesAsync(
                address.Street, address.City, address.PostalCode, address.Country, ct);

            if (coords is null)
                logger.LogWarning("Geocoding failed for updated address {AddressId} ({Street}, {City}, {Country}).",
                    address.Id, address.Street, address.City, address.Country);

            address.Latitude                = coords?.Latitude;
            address.Longitude               = coords?.Longitude;
            address.DistanceFromShopKm      = null;
            address.TravelTimeFromShopMinutes = null;
            address.TravelMetadataLocationId  = null;
        }

        await context.SaveChangesAsync(ct);
        return Unit.Value;
    }
}