
namespace Detailly.Application.Modules.Booking.Reviews.Commands.Delete;
public class DeleteReviewCommandValidator : AbstractValidator<DeleteReviewCommand>
{
    public DeleteReviewCommandValidator()
    {
        // Required ID (number)
        RuleFor(x => x.BookingId)
            .GreaterThan(0).WithMessage("Booking ID must be greater than zero.");
    }
}
