namespace Detailly.Application.Modules.Payment.Card.Commands.HandleStripeWebhook;

public sealed class HandleStripeWebhookCommandValidator : AbstractValidator<HandleStripeWebhookCommand>
{
    public HandleStripeWebhookCommandValidator()
    {
        RuleFor(x => x.Payload)
            .NotEmpty()
            .WithMessage("Webhook payload is required.");
    }
}
