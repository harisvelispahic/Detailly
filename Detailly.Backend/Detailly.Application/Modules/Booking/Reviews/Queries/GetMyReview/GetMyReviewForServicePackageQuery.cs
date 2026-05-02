namespace Detailly.Application.Modules.Booking.Reviews.Queries.GetMyReview;

public class GetMyReviewForServicePackageQuery : IRequest<GetMyReviewForServicePackageDto?>
{
    [JsonIgnore]
    public int ServicePackageId { get; set; }
}
