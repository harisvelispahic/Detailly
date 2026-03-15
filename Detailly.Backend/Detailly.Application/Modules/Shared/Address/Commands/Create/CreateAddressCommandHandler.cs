using Detailly.Domain.Entities.Shared;

namespace Detailly.Application.Modules.Shared.Address.Commands.Create;

public sealed class CreateAddressCommandHandler(
    IAppDbContext context,
    IAppCurrentUser appCurrentUser)
    : IRequestHandler<CreateAddressCommand, int>
{
    public async Task<int> Handle(CreateAddressCommand request, CancellationToken ct)
    {
        if (!appCurrentUser.IsAuthenticated || appCurrentUser.ApplicationUserId is null)
            throw new DetaillyUnauthorizedException("User is not authenticated.");

        var address = new AddressEntity
        {
            Street = request.Street.Trim(),
            City = request.City.Trim(),
            PostalCode = request.PostalCode.Trim(),
            Region = string.IsNullOrWhiteSpace(request.Region) ? null : request.Region.Trim(),
            Country = request.Country.Trim(),
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            ApplicationUserId = appCurrentUser.ApplicationUserId.Value
        };

        context.Addresses.Add(address);
        await context.SaveChangesAsync(ct);

        return address.Id;
    }
}