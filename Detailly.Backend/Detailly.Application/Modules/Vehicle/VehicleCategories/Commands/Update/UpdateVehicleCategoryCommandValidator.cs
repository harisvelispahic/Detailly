using FluentValidation;

namespace Detailly.Application.Modules.Vehicle.VehicleCategories.Commands.Update;

public class UpdateVehicleCategoryCommandValidator : AbstractValidator<UpdateVehicleCategoryCommand>
{
    public UpdateVehicleCategoryCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200).When(x => x.Name is not null);
        RuleFor(x => x.Description).MaximumLength(2000).When(x => x.Description is not null);
        RuleFor(x => x.BasePriceMultiplier).GreaterThan(0).When(x => x.BasePriceMultiplier is not null);
    }
}
