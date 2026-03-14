namespace Detailly.Application.Modules.Booking.ServicePackages.Queries.GetAvailableAddons;

public sealed class GetAvailableAddonsQueryHandler(IAppDbContext context)
    : IRequestHandler<GetAvailableAddonsQuery, List<GetAvailableAddonsQueryDto>>
{
    public async Task<List<GetAvailableAddonsQueryDto>> Handle(GetAvailableAddonsQuery request, CancellationToken ct)
    {
        var packageExists = await context.ServicePackages
            .AnyAsync(x => x.Id == request.ServicePackageId && !x.IsDeleted, ct);

        if (!packageExists)
            throw new DetaillyNotFoundException("Service package not found.");

        // base item ids for this package (exclude deleted assignments)
        var baseItemIds = await context.ServicePackageItemAssignments
            .Where(a => a.ServicePackageId == request.ServicePackageId && !a.IsDeleted)
            .Select(a => a.ServicePackageItemId)
            .ToListAsync(ct);

        // available add-ons: IsAddon=true, IsActive=true, not deleted, and not in base package
        var addons = await context.ServicePackageItems
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
            })
            .ToListAsync(ct);

        return addons;
    }
}