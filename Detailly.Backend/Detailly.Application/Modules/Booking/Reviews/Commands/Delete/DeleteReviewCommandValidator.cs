namespace Detailly.Application.Modules.Booking.Reviews.Commands.Delete;

public class DeleteReviewCommandValidator : AbstractValidator<DeleteReviewCommand>
{
    public DeleteReviewCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
    }
}
