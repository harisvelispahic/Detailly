
namespace Detailly.Application.Modules.Booking.Reviews.Queries.List;

public class ListReviewsQuery : IRequest<List<ListReviewsQueryDto>>
{
    public string? Search { get; init; }
}
