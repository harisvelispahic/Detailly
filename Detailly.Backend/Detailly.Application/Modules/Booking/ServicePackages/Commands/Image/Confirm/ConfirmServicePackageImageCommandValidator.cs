namespace Detailly.Application.Modules.Booking.ServicePackages.Commands.Image.Confirm;

public sealed class ConfirmServicePackageImageCommandValidator : AbstractValidator<ConfirmServicePackageImageCommand>
{
    public ConfirmServicePackageImageCommandValidator()
    {
        RuleFor(x => x.ServicePackageId)
            .GreaterThan(0)
            .WithMessage("Service package ID must be greater than zero.");

        RuleFor(x => x.PublicId)
            .NotEmpty()
            .WithMessage("Public ID is required.")
            .MaximumLength(500)
            .WithMessage("Public ID cannot exceed 500 characters.");

        RuleFor(x => x.SecureUrl)
            .NotEmpty()
            .WithMessage("Secure URL is required.")
            .MaximumLength(2000)
            .WithMessage("Secure URL cannot exceed 2000 characters.");

        RuleFor(x => x.FileHash)
            .NotEmpty()
            .WithMessage("File hash is required.")
            .MaximumLength(200)
            .WithMessage("File hash cannot exceed 200 characters.");
    }
}
