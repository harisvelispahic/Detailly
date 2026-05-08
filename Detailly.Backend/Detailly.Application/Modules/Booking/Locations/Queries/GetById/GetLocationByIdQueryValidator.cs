namespace Detailly.Application.Modules.Booking.Locations.Queries.GetById;

public sealed class GetLocationByIdQueryValidator : AbstractValidator<GetLocationByIdQuery>
{
    public GetLocationByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Location ID must be greater than zero.");
    }
}
