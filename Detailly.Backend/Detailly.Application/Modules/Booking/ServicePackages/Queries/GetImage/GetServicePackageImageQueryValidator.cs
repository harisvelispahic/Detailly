namespace Detailly.Application.Modules.Booking.ServicePackages.Queries.GetImage;

public sealed class GetServicePackageImageQueryValidator : AbstractValidator<GetServicePackageImageQuery>
{
    public GetServicePackageImageQueryValidator()
    {
        RuleFor(x => x.ServicePackageId)
            .GreaterThan(0)
            .WithMessage("Service package ID must be greater than zero.");

        RuleFor(x => x.ImageId)
            .GreaterThan(0)
            .WithMessage("Image ID must be greater than zero.");
    }
}
