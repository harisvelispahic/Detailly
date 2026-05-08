namespace Detailly.Application.Modules.Payment.Wallet.Commands.PayBooking;

public sealed class PayBookingWithWalletCommandValidator : AbstractValidator<PayBookingWithWalletCommand>
{
    public PayBookingWithWalletCommandValidator()
    {
        RuleFor(x => x.UserId)
            .GreaterThan(0)
            .WithMessage("User ID must be greater than zero.");

        RuleFor(x => x.BookingId)
            .GreaterThan(0)
            .WithMessage("Booking ID must be greater than zero.");
    }
}
