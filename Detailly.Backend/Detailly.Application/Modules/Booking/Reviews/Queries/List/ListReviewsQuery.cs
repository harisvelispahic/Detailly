namespace Detailly.Application.Modules.Booking.Reviews.Queries.List;

public class ListReviewsQuery : BasePagedQuery<ListReviewsQueryDto>
{
    public int? ServicePackageId { get; init; }
    public string? Sort { get; init; } = "newest"; // "newest" | "oldest"
}
