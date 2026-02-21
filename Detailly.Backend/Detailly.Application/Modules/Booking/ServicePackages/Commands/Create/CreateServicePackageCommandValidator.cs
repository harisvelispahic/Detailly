using FluentValidation;

namespace Detailly.Application.Modules.Booking.ServicePackages.Commands.Create;

public sealed class CreateServicePackageCommandValidator : AbstractValidator<CreateServicePackageCommand>
{
    public CreateServicePackageCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Description)
            .MaximumLength(2000)
            .When(x => x.Description is not null);

        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(0);


        When(x => x.ServicePackageItemIds is not null, () =>
        {
            RuleForEach(x => x.ServicePackageItemIds!)
                .GreaterThan(0);

            RuleFor(x => x.ServicePackageItemIds!)
                .Must(list => list.Distinct().Count() == list.Count)
                .WithMessage("ItemIds must not contain duplicates.");
        });
    }
}
