using FluentValidation;

namespace Detailly.Application.Modules.Booking.ServicePackages.Commands.Update;

public sealed class UpdateServicePackageCommandValidator : AbstractValidator<UpdateServicePackageCommand>
{
    public UpdateServicePackageCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0);

        When(x => x.Name is not null, () =>
        {
            RuleFor(x => x.Name!)
                .NotEmpty()
                .MaximumLength(200);
        });

        When(x => x.Description is not null, () =>
        {
            RuleFor(x => x.Description!)
                .MaximumLength(2000);
        });

        When(x => x.Price.HasValue, () =>
        {
            RuleFor(x => x.Price!.Value)
                .GreaterThanOrEqualTo(0);
        });

        When(x => x.EstimatedDurationHours.HasValue, () =>
        {
            RuleFor(x => x.EstimatedDurationHours!.Value)
                .GreaterThan(0);
        });

        When(x => x.ItemIds is not null, () =>
        {
            RuleForEach(x => x.ItemIds!)
                .GreaterThan(0);

            RuleFor(x => x.ItemIds!)
                .Must(list => list.Distinct().Count() == list.Count)
                .WithMessage("ItemIds must not contain duplicates.");
        });
    }
}
