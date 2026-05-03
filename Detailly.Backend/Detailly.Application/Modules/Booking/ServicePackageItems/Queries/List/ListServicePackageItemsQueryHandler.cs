namespace Detailly.Application.Modules.Booking.ServicePackageItems.Queries.List;

public sealed class ListServicePackageItemsQueryHandler(IAppDbContext ctx)
    : IRequestHandler<ListServicePackageItemsQuery, PageResult<ListServicePackageItemsQueryDto>>
{
    public async Task<PageResult<ListServicePackageItemsQueryDto>> Handle(
        ListServicePackageItemsQuery request, CancellationToken ct)
    {
        var q = ctx.ServicePackageItems
            .AsNoTracking()
            .Where(x => !x.IsDeleted && x.IsActive);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim();
            q = q.Where(x =>
                x.Name.Contains(search) ||
                (x.Description != null && x.Description.Contains(search)));
        }

        var projected = q
            .OrderBy(x => x.IsAddon)
            .ThenBy(x => x.Name)
            .Select(x => new ListServicePackageItemsQueryDto
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                Price = x.Price,
                DurationMinutes = x.DurationMinutes,
                RequiredEmployees = x.RequiredEmployees,
                IsAddon = x.IsAddon,
                IsActive = x.IsActive,
            });

        return await PageResult<ListServicePackageItemsQueryDto>
            .FromQueryableAsync(projected, request.Paging, ct);
    }
}
