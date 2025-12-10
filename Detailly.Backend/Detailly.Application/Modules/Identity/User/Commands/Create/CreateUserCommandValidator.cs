
using Detailly.Application.Modules.Identity.User.Commands.Create;

public sealed class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required.")
            .MaximumLength(100).WithMessage("First name can be at most 100 characters long.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required.")
            .MaximumLength(100).WithMessage("Last name can be at most 100 characters long.");

        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Username is required.")
            .MaximumLength(50).WithMessage("Username can be at most 50 characters long.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Email is not in a valid format.")
            .MaximumLength(200).WithMessage("Email can be at most 200 characters long.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters long.");

        RuleFor(x => x.Phone)
            .MaximumLength(30).When(x => !string.IsNullOrEmpty(x.Phone))
            .WithMessage("Phone can be at most 30 characters long.");

        RuleFor(x => x.CompanyName)
            .MaximumLength(200).When(x => !string.IsNullOrEmpty(x.CompanyName))
            .WithMessage("Company name can be at most 200 characters long.");

        RuleFor(x => x.IsFleet)
            .NotNull().WithMessage("Fleet flag is required.");
    }
}