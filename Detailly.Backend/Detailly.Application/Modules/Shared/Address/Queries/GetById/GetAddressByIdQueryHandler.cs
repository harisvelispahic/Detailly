namespace Detailly.Application.Modules.Shared.Address.Queries.GetById;

public sealed class GetAddressByIdQueryHandler(
    IAppDbContext context,
    IAppCurrentUser appCurrentUser)
    : IRequestHandler<GetAddressByIdQuery, GetAddressByIdQueryDto>
{
    public async Task<GetAddressByIdQueryDto> Handle(GetAddressByIdQuery request, CancellationToken ct)
    {
        if (!appCurrentUser.IsAuthenticated || appCurrentUser.ApplicationUserId is null)
            throw new DetaillyUnauthorizedException("User is not authenticated.");

        var userId = appCurrentUser.ApplicationUserId.Value;
        var isStaff = appCurrentUser.IsAdmin || appCurrentUser.IsManager;

        var address = await context.Addresses
            .AsNoTracking()
            .Where(a => a.Id == request.Id && (isStaff || a.ApplicationUserId == userId))
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

        if (address is null)
            throw new DetaillyNotFoundException($"Address with Id {request.Id} not found.");

        return address;
    }
}