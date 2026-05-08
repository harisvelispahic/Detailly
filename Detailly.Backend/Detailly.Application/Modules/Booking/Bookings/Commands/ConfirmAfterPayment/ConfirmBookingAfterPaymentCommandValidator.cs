namespace Detailly.Application.Modules.Booking.Bookings.Commands.ConfirmAfterPayment;

public sealed class ConfirmBookingAfterPaymentCommandValidator : AbstractValidator<ConfirmBookingAfterPaymentCommand>
{
    public ConfirmBookingAfterPaymentCommandValidator()
    {
        RuleFor(x => x.PaymentTransactionId)
            .GreaterThan(0)
            .WithMessage("Payment transaction ID must be greater than zero.");
    }
}
