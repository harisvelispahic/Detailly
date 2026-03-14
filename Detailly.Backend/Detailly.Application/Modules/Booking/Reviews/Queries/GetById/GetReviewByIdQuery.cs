namespace Detailly.Application.Modules.Booking.Reviews.Queries.GetById;
public class GetReviewByIdQuery : IRequest<GetReviewByIdQueryDto>
{
    public int Id { get; set; }
}
