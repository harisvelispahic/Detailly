namespace Detailly.Application.Modules.Booking.Reviews.Queries.ListMy;

public class ListMyReviewsQuery : BasePagedQuery<ListMyReviewsQueryDto>
{
    public string? Sort { get; init; } = "newest"; // "newest" | "oldest"
}
