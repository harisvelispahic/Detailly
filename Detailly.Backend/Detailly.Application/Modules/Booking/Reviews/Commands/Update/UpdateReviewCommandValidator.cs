
namespace Detailly.Application.Modules.Booking.Reviews.Commands.Update;

public class UpdateReviewCommandValidator : AbstractValidator<UpdateReviewCommand>
{
    public UpdateReviewCommandValidator()
    {
        RuleFor(x => x.BookingId)
            .GreaterThan(0);

        // Rating is optional but must be valid if supplied
        When(x => x.Rating.HasValue, () =>
        {
            RuleFor(x => x.Rating!.Value)
                .InclusiveBetween(1, 5);
        });

        // Description: optional but trimmed length check if provided
        When(x => x.Description is not null, () =>
        {
            RuleFor(x => x.Description!)
                .MaximumLength(2000);
        });

        // ValueForMoney: optional but must be 1–5 if provided
        When(x => x.ValueForMoney.HasValue, () =>
        {
            RuleFor(x => x.ValueForMoney!.Value)
                .InclusiveBetween(1, 5);
        });

        // Images validation
        When(x => x.Images is not null, () =>
        {
            RuleForEach(x => x.Images!)
                .ChildRules(img =>
                {
                    img.RuleFor(i => i.ImageUrl)
                        .NotEmpty()
                        .MaximumLength(500);

                    img.RuleFor(i => i.AltText)
                        .MaximumLength(200)
                        .When(i => i.AltText is not null);
                });
        });
    }
}