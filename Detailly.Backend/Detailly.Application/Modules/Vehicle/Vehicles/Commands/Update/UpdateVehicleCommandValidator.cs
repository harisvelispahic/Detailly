
namespace Detailly.Application.Modules.Vehicle.Vehicles.Commands.Update;

public sealed class UpdateVehicleCommandValidator : AbstractValidator<UpdateVehicleCommand>
{
    public UpdateVehicleCommandValidator()
    {
        // Required ID (number)
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Vehicle ID must be greater than zero.");

        // Optional Brand (string, use Custom for trimming)
        RuleFor(x => x.Brand)
            .Custom((value, context) =>
            {
                var trimmed = value?.Trim();
                if (!string.IsNullOrEmpty(trimmed) && trimmed.Length > 50)
                    context.AddFailure("Brand must be at most 50 characters long.");
            });

        // Optional Model (string)
        RuleFor(x => x.Model)
            .Custom((value, context) =>
            {
                var trimmed = value?.Trim();
                if (!string.IsNullOrEmpty(trimmed) && trimmed.Length > 50)
                    context.AddFailure("Model must be at most 50 characters long.");
            });

        // Optional YearOfManufacture (number)
        RuleFor(x => x.YearOfManufacture)
            .InclusiveBetween(1900, DateTime.UtcNow.AddYears(1).Year)
            .When(x => x.YearOfManufacture.HasValue)
            .WithMessage("Year of manufacture must be between 1900 and the current year.");

        // Optional VehicleCategoryId (number)
        RuleFor(x => x.VehicleCategoryId)
            .GreaterThan(0)
            .When(x => x.VehicleCategoryId.HasValue)
            .WithMessage("Vehicle category ID must be greater than zero.");

        // Optional LicencePlate (string)
        RuleFor(x => x.LicencePlate)
            .Custom((value, context) =>
            {
                var trimmed = value?.Trim();
                if (!string.IsNullOrEmpty(trimmed) && trimmed.Length > 15)
                    context.AddFailure("Licence plate must be at most 15 characters long.");
            });

        // Optional Notes (string)
        RuleFor(x => x.Notes)
            .Custom((value, context) =>
            {
                var trimmed = value?.Trim();
                if (!string.IsNullOrEmpty(trimmed) && trimmed.Length > 500)
                    context.AddFailure("Notes must be at most 500 characters long.");
            });
    }
}
