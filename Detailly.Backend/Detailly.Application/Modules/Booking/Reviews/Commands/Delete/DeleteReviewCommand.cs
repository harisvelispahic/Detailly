
namespace Detailly.Application.Modules.Booking.Reviews.Commands.Delete;
public class DeleteReviewCommand : IRequest<Unit>
{
    public required int BookingId { get; set; }
}
