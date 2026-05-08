namespace Detailly.Application.Modules.Vehicle.VehicleCategories.Queries.GetById;

public sealed class GetVehicleCategoryByIdQueryValidator : AbstractValidator<GetVehicleCategoryByIdQuery>
{
    public GetVehicleCategoryByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Vehicle category ID must be greater than zero.");
    }
}
