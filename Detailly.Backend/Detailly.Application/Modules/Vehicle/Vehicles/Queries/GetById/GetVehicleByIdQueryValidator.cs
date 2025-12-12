
namespace Detailly.Application.Modules.Vehicle.Vehicles.Queries.GetById;

public sealed class GetVehicleByIdQueryValidator : AbstractValidator<GetVehicleByIdQuery>
{
    public GetVehicleByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Vehicle ID must be greater than zero.");
    }
}