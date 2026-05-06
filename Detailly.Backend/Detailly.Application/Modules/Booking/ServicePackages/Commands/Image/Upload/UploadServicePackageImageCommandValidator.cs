namespace Detailly.Application.Modules.Booking.ServicePackages.Commands.Image.Upload;

public sealed class UploadServicePackageImageCommandValidator : AbstractValidator<UploadServicePackageImageCommand>
{
    private static readonly string[] AllowedTypes = ["image/jpeg", "image/png", "image/webp", "image/gif"];
    private const long MaxBytes = 10 * 1024 * 1024; // 10 MB

    public UploadServicePackageImageCommandValidator()
    {
        RuleFor(x => x.ServicePackageId)
            .GreaterThan(0);

        RuleFor(x => x.FileStream)
            .NotNull().WithMessage("File is required.");

        RuleFor(x => x.FileSize)
            .GreaterThan(0).WithMessage("File must not be empty.")
            .LessThanOrEqualTo(MaxBytes).WithMessage("File must not exceed 10 MB.");

        RuleFor(x => x.ContentType)
            .NotEmpty()
            .Must(ct => AllowedTypes.Contains(ct.ToLower()))
            .WithMessage("Only JPEG, PNG, WebP and GIF images are allowed.");
    }
}
