using FluentValidation;

namespace Detailly.Application.Modules.Vehicle.VehicleCategories.Commands.Create;

public class CreateVehicleCategoryCommandValidator : AbstractValidator<CreateVehicleCategoryCommand>
{
    public CreateVehicleCategoryCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).MaximumLength(2000).When(x => x.Description is not null);
        RuleFor(x => x.BasePriceMultiplier).GreaterThan(0);
    }
}
