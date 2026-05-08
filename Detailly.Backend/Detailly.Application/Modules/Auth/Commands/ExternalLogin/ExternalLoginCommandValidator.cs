using System.Security.Claims;

namespace Detailly.Application.Modules.Auth.Commands.ExternalLogin;

public sealed class ExternalLoginCommandValidator : AbstractValidator<ExternalLoginCommand>
{
    public ExternalLoginCommandValidator()
    {
        RuleFor(x => x.Provider)
            .NotEmpty()
            .WithMessage("Provider is required.")
            .MaximumLength(50)
            .WithMessage("Provider cannot exceed 50 characters.");

        RuleFor(x => x.Principal)
            .NotNull()
            .WithMessage("Principal is required.");
    }
}
