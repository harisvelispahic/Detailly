using Detailly.Application.Abstractions.Booking;
using Detailly.Domain.Entities.Shared;
using Microsoft.Extensions.Logging;

namespace Detailly.Application.Modules.Shared.Address.Commands.Create;

public sealed class CreateAddressCommandHandler(
    IAppDbContext context,
    IAppAuthorizationService authService,
    IRoadDistanceService roadDistanceService,
    ILogger<CreateAddressCommandHandler> logger)
    : IRequestHandler<CreateAddressCommand, int>
{
    public async Task<int> Handle(CreateAddressCommand request, CancellationToken ct)
    {
        var userId = authService.RequireUserId();

        var street  = request.Street.Trim();
        var city    = request.City.Trim();
        var postal  = request.PostalCode.Trim();
        var country = request.Country.Trim();

        var coords = await roadDistanceService.GetCoordinatesAsync(street, city, postal, country, ct);

        if (coords is null)
            logger.LogWarning("Geocoding failed for new address ({Street}, {City}, {Country}).", street, city, country);

        var address = new AddressEntity
        {
            Street    = street,
            City      = city,
            PostalCode = postal,
            Region    = string.IsNullOrWhiteSpace(request.Region) ? null : request.Region.Trim(),
            Country   = country,
            Latitude  = coords?.Latitude,
            Longitude = coords?.Longitude,
            ApplicationUserId = userId
        };

        context.Addresses.Add(address);
        await context.SaveChangesAsync(ct);

        return address.Id;
    }
}