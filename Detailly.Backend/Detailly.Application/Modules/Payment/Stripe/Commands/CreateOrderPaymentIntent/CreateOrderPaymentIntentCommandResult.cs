namespace Detailly.Application.Modules.Payment.Stripe.Commands.CreateOrderPaymentIntent;

public sealed class CreateOrderPaymentIntentResult
{
    public required string ClientSecret { get; init; }
}