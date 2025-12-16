using Microsoft.EntityFrameworkCore;

namespace Detailly.Application.Modules.Shared.Address.Queries.List;

public class ListAddressesQueryHandler(IAppDbContext ctx)
    : IRequestHandler<ListAddressesQuery, PageResult<ListAddressesQueryDto>>
{
    public async Task<PageResult<ListAddressesQueryDto>> Handle(
        ListAddressesQuery request, CancellationToken ct)
    {
        var q = ctx.Addresses.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim();
            q = q.Where(x =>
                x.Street!.Contains(search) ||
                x.City!.Contains(search) ||
                x.PostalCode!.Contains(search) ||
                x.Country!.Contains(search));
        }

        var projectedQuery =
            q.OrderBy(x => x.City)
             .ThenBy(x => x.Street)
             .Select(x => new ListAddressesQueryDto
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

        return await PageResult<ListAddressesQueryDto>.FromQueryableAsync(projectedQuery, request.Paging, ct);
    }
}
