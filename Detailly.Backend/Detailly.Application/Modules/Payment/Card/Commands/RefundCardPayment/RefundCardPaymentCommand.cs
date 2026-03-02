
namespace Detailly.Application.Modules.Payment.Card.Commands.RefundCardPayment;

public record RefundCardPaymentCommand(
    int PaymentTransactionId,
    decimal Amount
) : IRequest<Unit>;