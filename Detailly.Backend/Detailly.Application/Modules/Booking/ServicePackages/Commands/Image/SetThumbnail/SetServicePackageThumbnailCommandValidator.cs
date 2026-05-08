namespace Detailly.Application.Modules.Booking.ServicePackages.Commands.Image.SetThumbnail;

public sealed class SetServicePackageThumbnailCommandValidator : AbstractValidator<SetServicePackageThumbnailCommand>
{
    public SetServicePackageThumbnailCommandValidator()
    {
        RuleFor(x => x.ServicePackageId)
            .GreaterThan(0)
            .WithMessage("Service package ID must be greater than zero.");

        RuleFor(x => x.ImageId)
            .GreaterThan(0)
            .WithMessage("Image ID must be greater than zero.");
    }
}
