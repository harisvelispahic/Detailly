using FluentValidation;

namespace Detailly.Application.Modules.Identity.User.Commands.Update;

public sealed class UpdateUserCommandValidator
    : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("User id is required.");

        When(x => x.FirstName != null, () =>
        {
            RuleFor(x => x.FirstName!)
                .NotEmpty()
                .MaximumLength(100);
        });

        When(x => x.LastName != null, () =>
        {
            RuleFor(x => x.LastName!)
                .NotEmpty()
                .MaximumLength(100);
        });

        When(x => x.Username != null, () =>
        {
            RuleFor(x => x.Username!)
                .NotEmpty()
                .MaximumLength(50);
        });

        When(x => x.Email != null, () =>
        {
            RuleFor(x => x.Email!)
                .NotEmpty()
                .EmailAddress()
                .MaximumLength(200);
        });

        When(x => x.Phone != null, () =>
        {
            RuleFor(x => x.Phone!)
                .MaximumLength(30);
        });

        When(x => x.CompanyName != null, () =>
        {
            RuleFor(x => x.CompanyName!)
                .MaximumLength(200);
        });

        // Address validation (only if object provided)
        When(x => x.Address != null, () =>
        {
            RuleFor(x => x.Address!.Street)
                .MaximumLength(200)
                .When(x => x.Address!.Street != null);

            RuleFor(x => x.Address!.City)
                .MaximumLength(100)
                .When(x => x.Address!.City != null);

            RuleFor(x => x.Address!.Region)
                .MaximumLength(100)
                .When(x => x.Address!.Region != null);

            RuleFor(x => x.Address!.PostalCode)
                .MaximumLength(20)
                .When(x => x.Address!.PostalCode != null);

            RuleFor(x => x.Address!.Country)
                .MaximumLength(100)
                .When(x => x.Address!.Country != null);

            RuleFor(x => x.Address!.Latitude)
                .InclusiveBetween(-90, 90)
                .When(x => x.Address!.Latitude.HasValue);

            RuleFor(x => x.Address!.Longitude)
                .InclusiveBetween(-180, 180)
                .When(x => x.Address!.Longitude.HasValue);
        });

        // Image validation
        When(x => x.Image != null, () =>
        {
            RuleFor(x => x.Image!.ImageUrl)
                .NotEmpty()
                .MaximumLength(500)
                .When(x => x.Image!.ImageUrl != null);
        });
    }
}
