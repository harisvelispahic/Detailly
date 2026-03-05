namespace Detailly.Application.Modules.Payment.Stripe.Commands.CreateOrderPaymentIntent;

public record CreateOrderPaymentIntentCommand(int OrderId)
    : IRequest<CreateOrderPaymentIntentResult>;