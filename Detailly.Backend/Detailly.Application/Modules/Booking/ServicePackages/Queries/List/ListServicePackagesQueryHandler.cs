using Detailly.Domain.Common.Enums;

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

        var total = await q.CountAsync(ct);

        // Pass 1: fetch paged packages with scalar fields only (no collection navigation)
        var pagedPackages = await q
            .OrderBy(x => x.Name)
            .Skip(request.Paging.SkipCount)
            .Take(request.Paging.PageSize)
            .Select(sp => new
            {
                sp.Id,
                sp.Name,
                sp.Description,
                sp.Price,
                EstimatedDurationMinutes = ctx.ServicePackageItemAssignments
                    .Where(a => a.ServicePackageId == sp.Id && !a.IsDeleted && !a.ServicePackageItem.IsDeleted)
                    .Sum(a => (int?)a.ServicePackageItem.DurationMinutes) ?? sp.BaseDurationMinutes ?? 0,
                AverageRating = ctx.Reviews
                    .Where(r => r.ServicePackageId == sp.Id && !r.IsDeleted)
                    .Select(r => (decimal?)r.Rating)
                    .Average(),
                ReviewCount = ctx.Reviews
                    .Count(r => r.ServicePackageId == sp.Id && !r.IsDeleted),
                LikeCount = ctx.Reactions
                    .Count(r => r.ServicePackageId == sp.Id && r.ReactionType == ReactionType.Like),
                DislikeCount = ctx.Reactions
                    .Count(r => r.ServicePackageId == sp.Id && r.ReactionType == ReactionType.Dislike),
                ThumbnailUrl = ctx.Images
                    .Where(i => i.ServicePackageId == sp.Id && i.IsThumbnail)
                    .Select(i => (string?)i.ImageUrl)
                    .FirstOrDefault(),
            })
            .ToListAsync(ct);

        var packageIds = pagedPackages.Select(p => p.Id).ToList();

        // Pass 2: fetch all items for the current page in one query
        var allItems = await ctx.ServicePackageItemAssignments
            .AsNoTracking()
            .Where(a => packageIds.Contains(a.ServicePackageId) && !a.IsDeleted && !a.ServicePackageItem.IsDeleted)
            .Select(a => new
            {
                a.ServicePackageId,
                Item = new ListServicePackagesQueryDtoItem
                {
                    Id = a.ServicePackageItem.Id,
                    Name = a.ServicePackageItem.Name,
                    Price = a.ServicePackageItem.Price,
                    Description = a.ServicePackageItem.Description
                }
            })
            .ToListAsync(ct);

        var itemsByPackage = allItems
            .GroupBy(x => x.ServicePackageId)
            .ToDictionary(g => g.Key, g => g.Select(x => x.Item).ToList());

        var dtos = pagedPackages.Select(p => new ListServicePackagesQueryDto
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            Price = p.Price,
            EstimatedDurationMinutes = p.EstimatedDurationMinutes,
            AverageRating = p.AverageRating,
            ReviewCount = p.ReviewCount,
            LikeCount = p.LikeCount,
            DislikeCount = p.DislikeCount,
            ThumbnailUrl = p.ThumbnailUrl,
            Items = itemsByPackage.TryGetValue(p.Id, out var items) ? items : []
        }).ToList();

        return new PageResult<ListServicePackagesQueryDto> { Total = total, Items = dtos };
    }
}
