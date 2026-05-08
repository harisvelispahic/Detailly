namespace Detailly.Application.Modules.Payment.Wallet.Commands.TopUpByCard;

public sealed class CreateWalletTopUpCardIntentCommandValidator : AbstractValidator<CreateWalletTopUpCardIntentCommand>
{
    public CreateWalletTopUpCardIntentCommandValidator()
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
