namespace Detailly.Application.Modules.Sales.Orders.Commands.ConfirmAfterPayment;

public sealed class ConfirmOrderAfterPaymentCommandValidator : AbstractValidator<ConfirmOrderAfterPaymentCommand>
{
    public ConfirmOrderAfterPaymentCommandValidator()
    {
        RuleFor(x => x.PaymentTransactionId)
            .GreaterThan(0)
            .WithMessage("Payment transaction ID must be greater than zero.");
    }
}
