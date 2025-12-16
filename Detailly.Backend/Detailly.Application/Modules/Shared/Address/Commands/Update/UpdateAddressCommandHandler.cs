using Microsoft.EntityFrameworkCore;
using Detailly.Domain.Entities.Shared;

namespace Detailly.Application.Modules.Shared.Address.Commands.Update;

public sealed class UpdateAddressCommandHandler(IAppDbContext context)
    : IRequestHandler<UpdateAddressCommand, Unit>
{
    public async Task<Unit> Handle(UpdateAddressCommand request, CancellationToken ct)
    {
        var address = await context.Addresses
            .FirstOrDefaultAsync(a => a.Id == request.Id, ct);

        if (address == null)
            throw new DetaillyNotFoundException($"Address with Id {request.Id} not found.");

        if (request.Street != null)
            address.Street = request.Street.Trim();

        if (request.City != null)
            address.City = request.City.Trim();

        if (request.PostalCode != null)
            address.PostalCode = request.PostalCode.Trim();

        if (request.Region != null)
            address.Region = request.Region.Trim();

        if (request.Country != null)
            address.Country = request.Country.Trim();

        if (request.Latitude.HasValue)
            address.Latitude = request.Latitude.Value;

        if (request.Longitude.HasValue)
            address.Longitude = request.Longitude.Value;

        await context.SaveChangesAsync(ct);
        return Unit.Value;
    }
}
