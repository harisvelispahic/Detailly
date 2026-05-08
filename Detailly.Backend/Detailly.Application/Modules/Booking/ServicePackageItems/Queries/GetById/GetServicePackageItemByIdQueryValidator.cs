namespace Detailly.Application.Modules.Booking.ServicePackageItems.Queries.GetById;

public sealed class GetServicePackageItemByIdQueryValidator : AbstractValidator<GetServicePackageItemByIdQuery>
{
    public GetServicePackageItemByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Service package item ID must be greater than zero.");
    }
}
