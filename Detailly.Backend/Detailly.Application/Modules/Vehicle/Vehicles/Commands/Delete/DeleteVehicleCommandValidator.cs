
namespace Detailly.Application.Modules.Vehicle.Vehicles.Commands.Delete;

public sealed class DeleteVehicleCommandValidator : AbstractValidator<DeleteVehicleCommand>
{
    public DeleteVehicleCommandValidator()
    {
        // Required ID (number)
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Vehicle ID must be greater than zero.");
    }
}
