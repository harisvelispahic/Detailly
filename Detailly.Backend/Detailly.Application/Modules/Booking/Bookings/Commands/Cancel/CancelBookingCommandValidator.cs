namespace Detailly.Application.Modules.Booking.Bookings.Commands.Cancel;

public sealed class CancelBookingCommandValidator : AbstractValidator<CancelBookingCommand>
{
    public CancelBookingCommandValidator()
    {
        RuleFor(x => x.BookingId)
            .GreaterThan(0)
            .WithMessage("Booking ID must be greater than zero.");
    }
}
