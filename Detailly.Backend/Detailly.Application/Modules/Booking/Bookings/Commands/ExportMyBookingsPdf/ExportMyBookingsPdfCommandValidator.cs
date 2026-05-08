namespace Detailly.Application.Modules.Booking.Bookings.Commands.ExportMyBookingsPdf;

public sealed class ExportMyBookingsPdfCommandValidator : AbstractValidator<ExportMyBookingsPdfCommand>
{
    public ExportMyBookingsPdfCommandValidator()
    {
        RuleFor(x => x.StartDateUtc)
            .Must(d => d != default)
            .WithMessage("Start date must be provided.");

        RuleFor(x => x.EndDateUtc)
            .Must(d => d != default)
            .WithMessage("End date must be provided.");

        RuleFor(x => x)
            .Must(x => x.EndDateUtc >= x.StartDateUtc)
            .WithMessage("End date must not be before start date.");
    }
}
