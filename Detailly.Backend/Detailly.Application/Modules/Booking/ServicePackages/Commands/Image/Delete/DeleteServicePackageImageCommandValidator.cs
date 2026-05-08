namespace Detailly.Application.Modules.Booking.ServicePackages.Commands.Image.Delete;

public sealed class DeleteServicePackageImageCommandValidator : AbstractValidator<DeleteServicePackageImageCommand>
{
    public DeleteServicePackageImageCommandValidator()
    {
        RuleFor(x => x.ServicePackageId)
            .GreaterThan(0)
            .WithMessage("Service package ID must be greater than zero.");

        RuleFor(x => x.ImageId)
            .GreaterThan(0)
            .WithMessage("Image ID must be greater than zero.");
    }
}
