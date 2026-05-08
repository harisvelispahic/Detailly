namespace Detailly.Application.Modules.Booking.Bookings.Queries.ListForDate;

public sealed class ListBookingsForDateQueryValidator : AbstractValidator<ListBookingsForDateQuery>
{
    public ListBookingsForDateQueryValidator()
    {
        RuleFor(x => x.DateUtc)
            .Must(d => d != default)
            .WithMessage("DateUtc must be provided.");

        RuleFor(x => x.ShopLocationId)
            .GreaterThan(0)
            .WithMessage("Shop location ID must be greater than zero.");
    }
}
