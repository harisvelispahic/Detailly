namespace Detailly.Application.Modules.Payment.Wallet.Commands.RefundWalletPayment;

public sealed class RefundWalletPaymentCommandValidator : AbstractValidator<RefundWalletPaymentCommand>
{
    public RefundWalletPaymentCommandValidator()
    {
        RuleFor(x => x.PaymentTransactionId)
            .GreaterThan(0)
            .WithMessage("Payment transaction ID must be greater than zero.");

        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .WithMessage("Refund amount must be greater than zero.");
    }
}
