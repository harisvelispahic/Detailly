
namespace Detailly.Application.Modules.Booking.Reviews.Queries.GetById;
public class GetReviewByIdQueryHandler(IAppDbContext context) 
    : IRequestHandler<GetReviewByIdQuery, GetReviewByIdQueryDto>
{
    public async Task<GetReviewByIdQueryDto> Handle(GetReviewByIdQuery request, CancellationToken ct)
    {
        var review = await context.Reviews
            .Where(r => r.BookingId == request.Id)
            .FirstOrDefaultAsync(ct);

        var result = await context.Reviews
            .Where(r => r.BookingId == request.Id)
            .Select(r => new GetReviewByIdQueryDto
            {
                BookingId = r.BookingId,
                Rating = r.Rating,
                Description = r.Description,
                ValueForMoney = r.ValueForMoney,
                Images = r.Images.Select(i => new GetReviewByIdQueryDtoImage
                {
                    Id = i.Id,
                    ImageUrl = i.ImageUrl,
                    AltText = i.AltText,
                    IsThumbnail = i.IsThumbnail,
                    DisplayOrder = i.DisplayOrder
                }).ToList()
            })
            .FirstOrDefaultAsync(ct);

        if (review == null)
            throw new DetaillyNotFoundException($"Review with Id {request.Id} not found.");

        if (review.IsDeleted)
            throw new DetaillyBusinessRuleException("123", "Review does not exist.");


        return result;
    }
}
