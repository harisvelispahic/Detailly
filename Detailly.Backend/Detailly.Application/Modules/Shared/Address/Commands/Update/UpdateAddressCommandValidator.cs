using FluentValidation;

namespace Detailly.Application.Modules.Shared.Address.Commands.Update;

public sealed class UpdateAddressCommandValidator : AbstractValidator<UpdateAddressCommand>
{
    public UpdateAddressCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Address Id is required.");

        When(x => x.Street != null, () =>
        {
            RuleFor(x => x.Street!)
                .NotEmpty()
                .MaximumLength(250);
        });

        When(x => x.City != null, () =>
        {
            RuleFor(x => x.City!)
                .NotEmpty()
                .MaximumLength(100);
        });

        When(x => x.PostalCode != null, () =>
        {
            RuleFor(x => x.PostalCode!)
                .NotEmpty()
                .MaximumLength(20);
        });

        When(x => x.Region != null, () =>
        {
            RuleFor(x => x.Region!)
                .MaximumLength(100);
        });

        When(x => x.Country != null, () =>
        {
            RuleFor(x => x.Country!)
                .NotEmpty()
                .MaximumLength(100);
        });

        When(x => x.Latitude.HasValue, () =>
        {
            RuleFor(x => x.Latitude.Value)
                .InclusiveBetween(-90, 90);
        });

        When(x => x.Longitude.HasValue, () =>
        {
            RuleFor(x => x.Longitude.Value)
                .InclusiveBetween(-180, 180);
        });
    }
}
