namespace Detailly.Application.Modules.Booking.Bookings.Queries.GetById;

public sealed class GetBookingByIdQueryValidator : AbstractValidator<GetBookingByIdQuery>
{
    public GetBookingByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Booking ID must be greater than zero.");
    }
}
