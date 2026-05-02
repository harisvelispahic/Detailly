namespace Detailly.Application.Modules.Booking.Reviews.Queries.GetMyReview;

public class GetMyReviewForServicePackageDto
{
    public required int Id { get; init; }
    public required int Rating { get; init; }
    public required string? Description { get; init; }
    public required DateTime CreatedAtUtc { get; init; }
}
