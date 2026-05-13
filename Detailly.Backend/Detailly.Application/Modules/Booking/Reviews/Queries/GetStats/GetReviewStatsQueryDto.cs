namespace Detailly.Application.Modules.Booking.Reviews.Queries.GetStats;

public sealed class GetReviewStatsQueryDto
{
    public decimal AverageRating { get; set; }
    public int TotalCount { get; set; }
    public List<RatingDistributionDto> Distribution { get; set; } = new();
}

public sealed class RatingDistributionDto
{
    public int Stars { get; set; }
    public int Count { get; set; }
}
