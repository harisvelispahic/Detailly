namespace Detailly.Application.Modules.Booking.Locations.Queries.GetOpeningHours;

public sealed class GetLocationOpeningHoursQueryValidator : AbstractValidator<GetLocationOpeningHoursQuery>
{
    public GetLocationOpeningHoursQueryValidator()
    {
        RuleFor(x => x.LocationId)
            .GreaterThan(0)
            .WithMessage("Location ID must be greater than zero.");
    }
}
