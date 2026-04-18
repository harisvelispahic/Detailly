namespace Detailly.Application.Modules.Booking.ServicePackages.Queries.GetAvailableAddons;

public sealed class GetAvailableAddonsQueryHandler(IAppDbContext context)
    : IRequestHandler<GetAvailableAddonsQuery, PageResult<GetAvailableAddonsQueryDto>>
{
    public async Task<PageResult<GetAvailableAddonsQueryDto>> Handle(GetAvailableAddonsQuery request, CancellationToken ct)
    {
        var packageExists = await context.ServicePackages
            .AnyAsync(x => x.Id == request.ServicePackageId && !x.IsDeleted, ct);

        if (!packageExists)
            throw new DetaillyNotFoundException("Service package not found.");

        // base item ids for this package (exclude deleted assignments)
        var baseItemIds = await context.ServicePackageItemAssignments
            .AsNoTracking()
            .Where(a => a.ServicePackageId == request.ServicePackageId && !a.IsDeleted)
            .Select(a => a.ServicePackageItemId)
            .ToListAsync(ct);

        // available add-ons: IsAddon=true, IsActive=true, not deleted, and not in base package
        var addons = context.ServicePackageItems
            .AsNoTracking()
            .Where(i => !i.IsDeleted
                        && i.IsAddon
                        && i.IsActive
                        && !baseItemIds.Contains(i.Id))
            .OrderBy(i => i.Name)
            .Select(i => new GetAvailableAddonsQueryDto
            {
                Id = i.Id,
                Name = i.Name,
                Description = i.Description,
                Price = i.Price,
                DurationMinutes = i.DurationMinutes,
                RequiredEmployees = i.RequiredEmployees
            });

        return await PageResult<GetAvailableAddonsQueryDto>.FromQueryableAsync(addons, request.Paging, ct);
    }
}
