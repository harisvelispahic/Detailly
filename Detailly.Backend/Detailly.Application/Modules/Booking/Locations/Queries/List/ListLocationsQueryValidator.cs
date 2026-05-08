namespace Detailly.Application.Modules.Booking.Locations.Queries.List;

public sealed class ListLocationsQueryValidator : AbstractValidator<ListLocationsQuery>
{
    public ListLocationsQueryValidator()
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
