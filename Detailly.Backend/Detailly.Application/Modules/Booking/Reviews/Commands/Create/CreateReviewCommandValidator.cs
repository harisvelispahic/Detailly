namespace Detailly.Application.Modules.Booking.Reviews.Commands.Create;

public sealed class CreateReviewCommandValidator : AbstractValidator<CreateReviewCommand>
{
    public CreateReviewCommandValidator()
    {
        RuleFor(x => x.BookingId)
            .GreaterThan(0);

        RuleFor(x => x.Rating)
            .InclusiveBetween(1, 5)
            .WithMessage("Rating must be between 1 and 5.");

        RuleFor(x => x.Description)
            .MaximumLength(1000)
            .WithMessage("Description can be at most 1000 characters.");
    }
}
