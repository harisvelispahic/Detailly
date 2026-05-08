namespace Detailly.Application.Modules.Payment.Card.Commands.CreateBookingPaymentIntent;

public sealed class CreateBookingPaymentIntentCommandValidator : AbstractValidator<CreateBookingPaymentIntentCommand>
{
    public CreateBookingPaymentIntentCommandValidator()
    {
        RuleFor(x => x.UserId)
            .GreaterThan(0)
            .WithMessage("User ID must be greater than zero.");

        RuleFor(x => x.BookingId)
            .GreaterThan(0)
            .WithMessage("Booking ID must be greater than zero.");
    }
}
