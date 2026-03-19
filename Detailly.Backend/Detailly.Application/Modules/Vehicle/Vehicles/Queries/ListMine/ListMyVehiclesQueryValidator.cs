namespace Detailly.Application.Modules.Vehicle.Vehicles.Queries.ListMine;

public sealed class ListMyVehiclesQueryValidator : AbstractValidator<ListMyVehiclesQuery>
{
    public ListMyVehiclesQueryValidator()
    {
        RuleFor(x => x.Search)
            .Custom((value, context) =>
            {
                var trimmed = value?.Trim();
                if (!string.IsNullOrEmpty(trimmed) && trimmed.Length > 50)
                {
                    context.AddFailure("Search must be at most 50 characters long.");
                }
            });
    }
}