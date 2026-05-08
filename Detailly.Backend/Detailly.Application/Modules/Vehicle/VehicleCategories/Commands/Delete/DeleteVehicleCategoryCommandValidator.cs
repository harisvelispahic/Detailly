namespace Detailly.Application.Modules.Vehicle.VehicleCategories.Commands.Delete;

public sealed class DeleteVehicleCategoryCommandValidator : AbstractValidator<DeleteVehicleCategoryCommand>
{
    public DeleteVehicleCategoryCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Vehicle category ID must be greater than zero.");
    }
}
