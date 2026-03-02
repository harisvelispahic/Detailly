
using Microsoft.EntityFrameworkCore;

namespace Detailly.Application.Modules.Booking.Locations.Queries.GetById;

public sealed class GetLocationByIdQueryHandler(IAppDbContext context)
    : IRequestHandler<GetLocationByIdQuery, GetLocationByIdQueryDto>
{
    public async Task<GetLocationByIdQueryDto> Handle(GetLocationByIdQuery request, CancellationToken ct)
    {
        var dto = await context.Locations
            .AsNoTracking()
            .Where(l => l.Id == request.Id && !l.IsDeleted)
            .Select(l => new GetLocationByIdQueryDto
            {
                Id = l.Id,
                Name = l.Name,
                Description = l.Description,
                LocationType = l.LocationType,
                TotalBays = l.TotalBays,
                AddressId = l.AddressId,
                Address = new GetLocationByIdQueryDto.AddressDto
                {
                    Id = l.Address.Id,
                    Street = l.Address.Street,
                    City = l.Address.City,
                    PostalCode = l.Address.PostalCode,
                    Region = l.Address.Region,
                    Country = l.Address.Country,
                    Latitude = l.Address.Latitude,
                    Longitude = l.Address.Longitude
                }
            })
            .FirstOrDefaultAsync(ct);

        if (dto is null)
            throw new DetaillyNotFoundException("Location not found.");

        return dto;
    }
}