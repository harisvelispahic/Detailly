namespace Detailly.Application.Modules.Booking.ServicePackages.Queries.DownloadImage;

public sealed class DownloadServicePackageImageQueryValidator : AbstractValidator<DownloadServicePackageImageQuery>
{
    public DownloadServicePackageImageQueryValidator()
    {
        RuleFor(x => x.ServicePackageId)
            .GreaterThan(0)
            .WithMessage("Service package ID must be greater than zero.");

        RuleFor(x => x.ImageId)
            .GreaterThan(0)
            .WithMessage("Image ID must be greater than zero.");
    }
}
