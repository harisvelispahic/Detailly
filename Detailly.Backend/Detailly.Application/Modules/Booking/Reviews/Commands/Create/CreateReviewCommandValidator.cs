
namespace Detailly.Application.Modules.Booking.Reviews.Commands.Create;

public sealed class CreateReviewCommandValidator
    : AbstractValidator<CreateReviewCommand>
{
    public CreateReviewCommandValidator()
    {
        // BookingId
        RuleFor(x => x.BookingId)
            .GreaterThan(0).WithMessage("BookingId must be a positive number.");

        // Rating
        RuleFor(x => x.Rating)
            .InclusiveBetween(1, 5)
            .WithMessage("Rating must be between 1 and 5.");

        // Description (optional)
        RuleFor(x => x.Description)
            .MaximumLength(1000)
            .WithMessage("Description can be at most 1000 characters long.");

        // ValueForMoney (optional)
        RuleFor(x => x.ValueForMoney)
            .InclusiveBetween(1, 5)
            .When(x => x.ValueForMoney.HasValue)
            .WithMessage("ValueForMoney must be between 1 and 5.");

        // Images list validation
        RuleForEach(x => x.Images)
            .SetValidator(new CreateReviewCommandImageValidator());

        // Optional: limit total number of images
        RuleFor(x => x.Images)
            .Must(list => list == null || list.Count <= 10)
            .WithMessage("A maximum of 10 images is allowed.");
    }
}

public sealed class CreateReviewCommandImageValidator
    : AbstractValidator<CreateReviewCommandImage>
{
    public CreateReviewCommandImageValidator()
    {
        RuleFor(x => x.ImageUrl)
            .NotEmpty().WithMessage("ImageUrl is required.")
            .MaximumLength(500).WithMessage("ImageUrl can be at most 500 characters long.");

        RuleFor(x => x.AltText)
            .MaximumLength(200)
            .WithMessage("AltText can be at most 200 characters long.");

        RuleFor(x => x.DisplayOrder)
            .GreaterThanOrEqualTo(0)
            .WithMessage("DisplayOrder must be non-negative.");
    }
}
