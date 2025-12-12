
namespace Detailly.Application.Modules.Booking.Reviews.Queries.List;
public class ListReviewsQueryHandler(IAppDbContext ctx)
    : IRequestHandler<ListReviewsQuery, List<ListReviewsQueryDto>>
{
    public async Task<List<ListReviewsQueryDto>> Handle(ListReviewsQuery request, CancellationToken ct)
    {
        var q = ctx.Reviews.AsNoTracking()
            .Where(x => x.IsDeleted == false);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            q = q.Where(x => x.Description.Contains(request.Search));
        }

        var reviews = q.OrderBy(x => x.CreatedAtUtc)
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

        return await reviews.ToListAsync(ct);
    }
}
