namespace Detailly.Application.Modules.Payment.Card.Commands.RefundStripePayment;

public sealed class RefundStripePaymentCommandValidator : AbstractValidator<RefundStripePaymentCommand>
{
    public RefundStripePaymentCommandValidator()
    {
        RuleFor(x => x.PaymentTransactionId)
            .GreaterThan(0)
            .WithMessage("Payment transaction ID must be greater than zero.");

        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .WithMessage("Refund amount must be greater than zero.");
    }
}
