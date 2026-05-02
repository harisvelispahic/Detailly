namespace Detailly.Application.Modules.Booking.Reviews.Queries.GetMyReview;

public class GetMyReviewForServicePackageQueryHandler(IAppDbContext context, IAppCurrentUser currentUser)
    : IRequestHandler<GetMyReviewForServicePackageQuery, GetMyReviewForServicePackageDto?>
{
    public async Task<GetMyReviewForServicePackageDto?> Handle(
        GetMyReviewForServicePackageQuery request, CancellationToken ct)
    {
        if (currentUser.ApplicationUserId is null)
            return null;

        return await context.Reviews
            .Where(r =>
                r.CustomerId == currentUser.ApplicationUserId.Value
                && r.ServicePackageId == request.ServicePackageId
                && !r.IsDeleted)
            .Select(r => new GetMyReviewForServicePackageDto
            {
                Id = r.Id,
                Rating = r.Rating,
                Description = r.Description,
                CreatedAtUtc = r.CreatedAtUtc,
            })
            .FirstOrDefaultAsync(ct);
    }
}
