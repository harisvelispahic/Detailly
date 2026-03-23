namespace Detailly.Application.Modules.Booking.Reviews.Commands.Delete;
public class DeleteReviewCommand : IRequest<Unit>
{
    [JsonIgnore]
    public int BookingId { get; set; }
}
