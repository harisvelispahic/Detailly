
namespace Detailly.Application.Modules.Vehicle.Vehicles.Queries.List;

public sealed class ListVehiclesQueryValidator : AbstractValidator<ListVehiclesQuery>
{
    public ListVehiclesQueryValidator()
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