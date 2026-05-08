namespace Detailly.Application.Modules.Booking.ServicePackages.Queries.GetAvailableAddons;

public sealed class GetAvailableAddonsQueryValidator : AbstractValidator<GetAvailableAddonsQuery>
{
    public GetAvailableAddonsQueryValidator()
    {
        RuleFor(x => x.ServicePackageId)
            .GreaterThan(0)
            .WithMessage("Service package ID must be greater than zero.");
    }
}
