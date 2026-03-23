namespace Detailly.Application.Modules.Booking.Reviews.Queries.List;

public class ListReviewsQuery : BasePagedQuery<ListReviewsQueryDto>
{
    public string? Search { get; init; }
}
