namespace Detailly.Application.Modules.Booking.ServicePackages.Queries.GetUploadParams;

public sealed class GetServicePackageUploadParamsQueryValidator : AbstractValidator<GetServicePackageUploadParamsQuery>
{
    public GetServicePackageUploadParamsQueryValidator()
    {
        RuleFor(x => x.ServicePackageId)
            .GreaterThan(0)
            .WithMessage("Service package ID must be greater than zero.");
    }
}
