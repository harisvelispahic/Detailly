namespace Detailly.Application.Modules.Booking.Bookings.Queries.ExportMyBookings;

public sealed class ExportMyBookingsQueryValidator : AbstractValidator<ExportMyBookingsQuery>
{
    public ExportMyBookingsQueryValidator()
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
