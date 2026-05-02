namespace Detailly.Application.Modules.Booking.Reviews.Commands.Create;

public class CreateReviewCommand : IRequest<int>
{
    [JsonIgnore]
    public int BookingId { get; set; }
    public required int Rating { get; set; }
    public string? Description { get; set; }
}
