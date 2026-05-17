namespace Detailly.Application.Modules.Auth.Commands.CompleteOAuthSetup;

public sealed class CompleteOAuthSetupCommandValidator : AbstractValidator<CompleteOAuthSetupCommand>
{
    public CompleteOAuthSetupCommandValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Username is required.")
            .MinimumLength(3).WithMessage("Username must be at least 3 characters.")
            .MaximumLength(50).WithMessage("Username cannot exceed 50 characters.")
            .Matches(@"^[a-zA-Z0-9._-]+$").WithMessage("Username may only contain letters, digits, dots, hyphens, and underscores.");

        RuleFor(x => x.Phone)
            .MaximumLength(20).WithMessage("Phone number is too long.")
            .When(x => x.Phone != null);

        RuleFor(x => x.CompanyName)
            .MaximumLength(200).WithMessage("Company name cannot exceed 200 characters.")
            .When(x => x.CompanyName != null);
    }
}
