namespace Detailly.Application.Modules.Booking.Reviews.Queries.GetById;

public class GetReviewByIdQueryHandler(IAppDbContext context)
    : IRequestHandler<GetReviewByIdQuery, GetReviewByIdQueryDto>
{
    public async Task<GetReviewByIdQueryDto> Handle(GetReviewByIdQuery request, CancellationToken ct)
    {
        var result = await context.Reviews
            .Where(r => r.Id == request.Id && !r.IsDeleted)
            .Select(r => new GetReviewByIdQueryDto
            {
                Id = r.Id,
                BookingId = r.BookingId,
                ServicePackageId = r.ServicePackageId,
                Rating = r.Rating,
                Description = r.Description,
                CreatedAtUtc = r.CreatedAtUtc,
            })
            .FirstOrDefaultAsync(ct);

        if (result is null)
            throw new DetaillyNotFoundException($"Review {request.Id} not found.");

        return result;
    }
}
