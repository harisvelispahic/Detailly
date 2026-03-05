namespace Detailly.Application.Modules.Payment.Card.Commands.RefundStripePayment;

public record RefundStripePaymentCommand(
    int PaymentTransactionId,
    decimal Amount
) : IRequest<Unit>;