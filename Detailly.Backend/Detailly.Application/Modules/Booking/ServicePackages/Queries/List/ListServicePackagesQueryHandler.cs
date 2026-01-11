using Microsoft.EntityFrameworkCore;

namespace Detailly.Application.Modules.Booking.ServicePackages.Queries.List;

public class ListServicePackagesQueryHandler(IAppDbContext ctx)
    : IRequestHandler<ListServicePackagesQuery, PageResult<ListServicePackagesQueryDto>>
{
    public async Task<PageResult<ListServicePackagesQueryDto>> Handle(
        ListServicePackagesQuery request, CancellationToken ct)
    {
        var q = ctx.ServicePackages
            .AsNoTracking()
            .Where(x => !x.IsDeleted);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim();
            q = q.Where(x =>
                x.Name.Contains(search) ||
                (x.Description != null && x.Description.Contains(search)));
        }

        var projected = q
            .OrderBy(x => x.Name)
            .Select(sp => new ListServicePackagesQueryDto
            {
                Id = sp.Id,
                Name = sp.Name,
                Description = sp.Description,
                Price = sp.Price,
                EstimatedDurationHours = sp.EstimatedDurationHours,

                Items = ctx.ServicePackageItemAssignments
                    .Where(a => a.ServicePackageId == sp.Id && !a.IsDeleted && !a.ServicePackageItem.IsDeleted)
                    .Select(a => new ListServicePackagesQueryDtoItem
                    {
                        Id = a.ServicePackageItem.Id,
                        Name = a.ServicePackageItem.Name,
                        Price = a.ServicePackageItem.Price,
                        Description = a.ServicePackageItem.Description
                    })
                    .ToList()
            });

        return await PageResult<ListServicePackagesQueryDto>
            .FromQueryableAsync(projected, request.Paging, ct);
    }
}
