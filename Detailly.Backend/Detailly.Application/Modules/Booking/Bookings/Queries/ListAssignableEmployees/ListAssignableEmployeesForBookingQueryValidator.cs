namespace Detailly.Application.Modules.Booking.Bookings.Queries.ListAssignableEmployees;

public sealed class ListAssignableEmployeesForBookingQueryValidator : AbstractValidator<ListAssignableEmployeesForBookingQuery>
{
    public ListAssignableEmployeesForBookingQueryValidator()
    {
        RuleFor(x => x.BookingId)
            .GreaterThan(0)
            .WithMessage("Booking ID must be greater than zero.");
    }
}
