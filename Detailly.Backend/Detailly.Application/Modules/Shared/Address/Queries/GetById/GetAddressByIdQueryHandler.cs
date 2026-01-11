using Detailly.Domain.Entities.Shared;
using Microsoft.EntityFrameworkCore;

namespace Detailly.Application.Modules.Shared.Address.Queries.GetById;

public class GetAddressByIdQueryHandler(IAppDbContext context)
    : IRequestHandler<GetAddressByIdQuery, GetAddressByIdQueryDto>
{
    public async Task<GetAddressByIdQueryDto> Handle(GetAddressByIdQuery request, CancellationToken ct)
    {
        var address = await context.Addresses
            .Where(a => a.Id == request.Id)
            .Select(a => new GetAddressByIdQueryDto
            {
                Id = a.Id,
                Street = a.Street!,
                City = a.City!,
                PostalCode = a.PostalCode!,
                Region = a.Region,
                Country = a.Country!,
                Latitude = a.Latitude,
                Longitude = a.Longitude
            })
            .FirstOrDefaultAsync(ct);

        if (address == null)
        {
            throw new DetaillyNotFoundException($"Address with Id {request.Id} not found.");
        }

        return address;
    }
}
