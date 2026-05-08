namespace Detailly.Application.Modules.Booking.Bookings.Queries.GetAvailability;

public sealed class GetAvailabilityQueryValidator : AbstractValidator<GetAvailabilityQuery>
{
    public GetAvailabilityQueryValidator()
    {
        RuleFor(x => x.DateUtc)
            .Must(d => d != default)
            .WithMessage("DateUtc must be provided.");

        RuleFor(x => x.ServicePackageId)
            .GreaterThan(0)
            .WithMessage("Service package ID must be greater than zero.");

        RuleFor(x => x.ShopLocationId)
            .GreaterThan(0)
            .WithMessage("Shop location ID must be greater than zero.");

        RuleForEach(x => x.AddonItemIds)
            .GreaterThan(0)
            .When(x => x.AddonItemIds is not null)
            .WithMessage("Each add-on item ID must be greater than zero.");
    }
}
