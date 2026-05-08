namespace Detailly.Application.Modules.Booking.Bookings.Commands.Complete;

public sealed class CompleteBookingCommandValidator : AbstractValidator<CompleteBookingCommand>
{
    public CompleteBookingCommandValidator()
    {
        RuleFor(x => x.BookingId)
            .GreaterThan(0)
            .WithMessage("Booking ID must be greater than zero.");
    }
}
