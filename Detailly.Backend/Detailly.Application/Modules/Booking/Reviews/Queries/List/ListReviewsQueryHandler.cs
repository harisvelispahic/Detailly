namespace Detailly.Application.Modules.Booking.Reviews.Queries.List;
public class ListReviewsQueryHandler(IAppDbContext ctx)
    : IRequestHandler<ListReviewsQuery, PageResult<ListReviewsQueryDto>>
{
    public async Task<PageResult<ListReviewsQueryDto>> Handle(ListReviewsQuery request, CancellationToken ct)
    {
        var q = ctx.Reviews.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            q = q.Where(x => x.Description.Contains(request.Search));
        }

        var projectedQuery = q.OrderBy(x => x.CreatedAtUtc)
            .Select(x => new ListReviewsQueryDto
            {
                BookingId = x.BookingId,
                Rating = x.Rating,
                Description = x.Description,
                ValueForMoney = x.ValueForMoney,
                Images = x.Images.Select(img => new ListReviewsQueryDtoImage
                {
                    Id = img.Id,
                    ImageUrl = img.ImageUrl,
                    AltText = img.AltText,
                    IsThumbnail = img.IsThumbnail,
                    DisplayOrder = img.DisplayOrder
                }).ToList()
            });

        return await PageResult<ListReviewsQueryDto>.FromQueryableAsync(projectedQuery, request.Paging, ct);

    }
}
