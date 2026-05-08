namespace Detailly.Application.Modules.Vehicle.VehicleCategories.Queries.List;

public sealed class ListVehicleCategoriesQueryValidator : AbstractValidator<ListVehicleCategoriesQuery>
{
    public ListVehicleCategoriesQueryValidator()
    {
        RuleFor(x => x.Search)
            .Custom((value, context) =>
            {
                var trimmed = value?.Trim();
                if (!string.IsNullOrEmpty(trimmed) && trimmed.Length > 50)
                    context.AddFailure("Search must be at most 50 characters long.");
            });
    }
}
