namespace Detailly.Application.Modules.Booking.Reviews.Queries.ListMy;

public class ListMyReviewsQueryHandler(IAppDbContext ctx, IAppCurrentUser currentUser)
    : IRequestHandler<ListMyReviewsQuery, PageResult<ListMyReviewsQueryDto>>
{
    public async Task<PageResult<ListMyReviewsQueryDto>> Handle(ListMyReviewsQuery request, CancellationToken ct)
    {
        if (currentUser.ApplicationUserId is null)
            return new PageResult<ListMyReviewsQueryDto> { Total = 0, Items = [] };

        var q = ctx.Reviews
            .AsNoTracking()
            .Where(r => !r.IsDeleted && r.CustomerId == currentUser.ApplicationUserId.Value);

        var ordered = request.Sort == "oldest"
            ? q.OrderBy(r => r.CreatedAtUtc)
            : q.OrderByDescending(r => r.CreatedAtUtc);

        var projected = ordered.Select(r => new ListMyReviewsQueryDto
        {
            Id = r.Id,
            ServicePackageId = r.ServicePackageId,
            ServicePackageName = r.ServicePackage.Name,
            Rating = r.Rating,
            Description = r.Description,
            AddonNames = ctx.BookingItems
                .Where(bi => bi.BookingId == r.BookingId && bi.IsAddon && !bi.IsDeleted)
                .Select(bi => bi.ServicePackageItem.Name)
                .ToList(),
            CreatedAtUtc = r.CreatedAtUtc,
            ModifiedAtUtc = r.ModifiedAtUtc,
        });

        return await PageResult<ListMyReviewsQueryDto>.FromQueryableAsync(projected, request.Paging, ct);
    }
}
