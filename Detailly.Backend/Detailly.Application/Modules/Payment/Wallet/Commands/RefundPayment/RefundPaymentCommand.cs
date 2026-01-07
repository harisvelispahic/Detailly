
namespace Detailly.Application.Modules.Payment.Wallet.Commands.RefundPayment;
public record RefundPaymentCommand(
    int PaymentTransactionId
) : IRequest<Unit>;
