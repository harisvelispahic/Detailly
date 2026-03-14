namespace Detailly.Application.Modules.Payment.Wallet.Commands.RefundWalletPayment;

public record RefundWalletPaymentCommand(
    int PaymentTransactionId,
    decimal Amount
) : IRequest<Unit>;