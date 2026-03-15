namespace Detailly.Application.Modules.Shared.Address.Queries.ListMine;

public sealed class ListMyAddressesQueryHandler(
    IAppDbContext context,
    IAppCurrentUser appCurrentUser)
    : IRequestHandler<ListMyAddressesQuery, PageResult<ListMyAddressesQueryDto>>
{
    public async Task<PageResult<ListMyAddressesQueryDto>> Handle(ListMyAddressesQuery request, CancellationToken ct)
    {
        if (!appCurrentUser.IsAuthenticated || appCurrentUser.ApplicationUserId is null)
            throw new DetaillyUnauthorizedException("User is not authenticated.");

        var userId = appCurrentUser.ApplicationUserId.Value;

        var q = context.Addresses
            .AsNoTracking()
            .Where(x => x.ApplicationUserId == userId);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim();

            q = q.Where(x =>
                (x.Street != null && x.Street.Contains(search)) ||
                (x.City != null && x.City.Contains(search)) ||
                (x.PostalCode != null && x.PostalCode.Contains(search)) ||
                (x.Country != null && x.Country.Contains(search)));
        }

        var projectedQuery = q
            .OrderBy(x => x.City)
            .ThenBy(x => x.Street)
            .Select(x => new ListMyAddressesQueryDto
            {
                Id = x.Id,
                Street = x.Street!,
                City = x.City!,
                PostalCode = x.PostalCode!,
                Region = x.Region,
                Country = x.Country!,
                Latitude = x.Latitude,
                Longitude = x.Longitude
            });

        return await PageResult<ListMyAddressesQueryDto>
            .FromQueryableAsync(projectedQuery, request.Paging, ct);
    }
}