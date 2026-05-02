namespace Detailly.Application.Modules.Booking.Reviews.Queries.GetById;

public class GetReviewByIdQuery : IRequest<GetReviewByIdQueryDto>
{
    [JsonIgnore]
    public int Id { get; set; }
}
