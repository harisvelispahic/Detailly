
namespace Detailly.Application.Modules.Vehicle.Vehicles.Commands.Create;

public sealed class CreateVehicleCommandValidator : AbstractValidator<CreateVehicleCommand>
{
    public CreateVehicleCommandValidator()
    {
        // required string fields

        RuleFor(x => x.Model)
            .Custom((value, context) =>
            {
                var trimmed = value?.Trim();
                if (string.IsNullOrEmpty(trimmed))
                {
                    context.AddFailure("Model is required.");
                }
                else if (trimmed.Length > 50)
                {
                    context.AddFailure("Model must be at most 50 characters long.");
                }
            });

        RuleFor(x => x.Brand)
            .Custom((value, context) =>
            {
                var trimmed = value?.Trim();
                if (string.IsNullOrEmpty(trimmed))
                {
                    context.AddFailure("Brand is required.");
                }
                else if (trimmed.Length > 50)
                {
                    context.AddFailure("Brand must be at most 50 characters long.");
                }
            });


        // required number fields

        RuleFor(x => x.YearOfManufacture)
            .InclusiveBetween(1900, DateTime.UtcNow.AddYears(1).Year)
            .WithMessage("Year of manufacture must be between 1900 and the current year.");

        //RuleFor(x => x.ApplicationUserId)
        //    .GreaterThan(0).WithMessage("Application user ID must be greater than zero.");

        RuleFor(x => x.VehicleCategoryId)
            .GreaterThan(0).WithMessage("Vehicle category ID must be greater than zero.");


        // optional string fields

        RuleFor(x => x.LicencePlate)
            .NotEmpty().WithMessage("Licence plate is required.")
            .Custom((value, context) =>
            {
                var trimmed = value?.Trim();
                if (!string.IsNullOrEmpty(trimmed) && trimmed.Length > 15)
                    context.AddFailure("Licence plate must be at most 15 characters long.");
            });

        RuleFor(x => x.Notes)
            .Custom((value, context) =>
            {
                var trimmed = value?.Trim();
                if (!string.IsNullOrEmpty(trimmed) && trimmed.Length > 500)
                {
                    context.AddFailure("Notes must be at most 500 characters long.");
                }
            });
    }
}

