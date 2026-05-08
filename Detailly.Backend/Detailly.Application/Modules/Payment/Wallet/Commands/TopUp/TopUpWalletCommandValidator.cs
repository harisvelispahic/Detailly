namespace Detailly.Application.Modules.Payment.Wallet.Commands.TopUp;

public sealed class TopUpWalletCommandValidator : AbstractValidator<TopUpWalletCommand>
{
    public TopUpWalletCommandValidator()
    {
        RuleFor(x => x.UserId)
            .GreaterThan(0)
            .WithMessage("User ID must be greater than zero.");

        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .WithMessage("Top-up amount must be greater than zero.");

        RuleFor(x => x.Description)
            .MaximumLength(500)
            .When(x => x.Description is not null)
            .WithMessage("Description cannot exceed 500 characters.");
    }
}
