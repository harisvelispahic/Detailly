using Detailly.Application.Modules.Payment.Stripe.Commands.CreateOrderPaymentIntent;

namespace Detailly.Application.Modules.Payment.Card.Commands.CreateOrderPaymentIntent;

public sealed class CreateOrderPaymentIntentCommandValidator : AbstractValidator<CreateOrderPaymentIntentCommand>
{
    public CreateOrderPaymentIntentCommandValidator()
    {
        RuleFor(x => x.OrderId)
            .GreaterThan(0)
            .WithMessage("Order ID must be greater than zero.");
    }
}
