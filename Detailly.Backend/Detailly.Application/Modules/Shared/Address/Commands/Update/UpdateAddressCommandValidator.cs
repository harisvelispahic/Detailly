namespace Detailly.Application.Modules.Shared.Address.Commands.Update;

public sealed class UpdateAddressCommandValidator : AbstractValidator<UpdateAddressCommand>
{
    public UpdateAddressCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Address id must be greater than 0.");

        RuleFor(x => x.Street)
            .Must(x => x == null || !string.IsNullOrWhiteSpace(x))
            .WithMessage("Street cannot be empty or whitespace only.")
            .MaximumLength(250).WithMessage("Street cannot exceed 250 characters.");

        RuleFor(x => x.City)
            .Must(x => x == null || !string.IsNullOrWhiteSpace(x))
            .WithMessage("City cannot be empty or whitespace only.")
            .MaximumLength(100).WithMessage("City cannot exceed 100 characters.");

        RuleFor(x => x.PostalCode)
            .Must(x => x == null || !string.IsNullOrWhiteSpace(x))
            .WithMessage("Postal code cannot be empty or whitespace only.")
            .MaximumLength(20).WithMessage("Postal code cannot exceed 20 characters.");

        RuleFor(x => x.Region)
            .Must(x => x == null || !string.IsNullOrWhiteSpace(x))
            .WithMessage("Region cannot be empty or whitespace only.")
            .MaximumLength(100).WithMessage("Region cannot exceed 100 characters.");

        RuleFor(x => x.Country)
            .Must(x => x == null || !string.IsNullOrWhiteSpace(x))
            .WithMessage("Country cannot be empty or whitespace only.")
            .MaximumLength(100).WithMessage("Country cannot exceed 100 characters.");

        RuleFor(x => x.Latitude)
            .InclusiveBetween(-90m, 90m)
            .When(x => x.Latitude.HasValue)
            .WithMessage("Latitude must be between -90 and 90.");

        RuleFor(x => x.Longitude)
            .InclusiveBetween(-180m, 180m)
            .When(x => x.Longitude.HasValue)
            .WithMessage("Longitude must be between -180 and 180.");
    }
}