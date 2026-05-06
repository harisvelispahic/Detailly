namespace Detailly.Application.Modules.Identity.User.Commands.Update;

public sealed class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("User id must be greater than 0.");

        RuleFor(x => x.FirstName)
            .NotEmpty()
            .WithMessage("First name cannot be empty.")
            .MaximumLength(100)
            .WithMessage("First name cannot exceed 100 characters.")
            .When(x => x.FirstName != null);

        RuleFor(x => x.LastName)
            .NotEmpty()
            .WithMessage("Last name cannot be empty.")
            .MaximumLength(100)
            .WithMessage("Last name cannot exceed 100 characters.")
            .When(x => x.LastName != null);

        RuleFor(x => x.Username)
            .NotEmpty()
            .WithMessage("Username cannot be empty.")
            .MaximumLength(50)
            .WithMessage("Username cannot exceed 50 characters.")
            .When(x => x.Username != null);

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email cannot be empty.")
            .EmailAddress()
            .WithMessage("Email must be a valid email address.")
            .MaximumLength(200)
            .WithMessage("Email cannot exceed 200 characters.")
            .When(x => x.Email != null);

        RuleFor(x => x.Phone)
            .MaximumLength(30)
            .WithMessage("Phone cannot exceed 30 characters.")
            .When(x => x.Phone != null);

        RuleFor(x => x.CompanyName)
            .MaximumLength(200)
            .WithMessage("Company name cannot exceed 200 characters.")
            .When(x => x.CompanyName != null);

    }
}
