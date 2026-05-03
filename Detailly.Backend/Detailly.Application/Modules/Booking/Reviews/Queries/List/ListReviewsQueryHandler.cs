namespace Detailly.Application.Modules.Booking.Reviews.Queries.List;

public class ListReviewsQueryHandler(IAppDbContext ctx)
    : IRequestHandler<ListReviewsQuery, PageResult<ListReviewsQueryDto>>
{
    public async Task<PageResult<ListReviewsQueryDto>> Handle(ListReviewsQuery request, CancellationToken ct)
    {
        var q = ctx.Reviews.AsNoTracking().Where(r => !r.IsDeleted);

        if (request.ServicePackageId.HasValue)
            q = q.Where(r => r.ServicePackageId == request.ServicePackageId.Value);

        var ordered = request.Sort == "oldest"
            ? q.OrderBy(r => r.CreatedAtUtc)
            : q.OrderByDescending(r => r.CreatedAtUtc);

        var projected = ordered.Select(r => new ListReviewsQueryDto
        {
            Id = r.Id,
            ServicePackageId = r.ServicePackageId,
            ServicePackageName = r.ServicePackage.Name,
            CustomerFullName = r.Customer.FirstName + " " + r.Customer.LastName,
            CustomerInitials = r.Customer.FirstName.Substring(0, 1) + r.Customer.LastName.Substring(0, 1),
            Rating = r.Rating,
            Description = r.Description,
            AddonNames = ctx.BookingItems
                .Where(bi => bi.BookingId == r.BookingId && bi.IsAddon && !bi.IsDeleted)
                .Select(bi => bi.ServicePackageItem.Name)
                .ToList(),
            CreatedAtUtc = r.CreatedAtUtc,
            ModifiedAtUtc = r.ModifiedAtUtc,
        });

        return await PageResult<ListReviewsQueryDto>.FromQueryableAsync(projected, request.Paging, ct);
    }
}
