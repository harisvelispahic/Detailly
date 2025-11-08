
namespace Detailly.Application.Modules.Vehicle.Vehicles.Queries.GetById;

public sealed class ListVehiclesQueryValidator : AbstractValidator<GetVehicleByIdQuery>
{
    public ListVehiclesQueryValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Vehicle ID must be greater than zero.");
    }
}