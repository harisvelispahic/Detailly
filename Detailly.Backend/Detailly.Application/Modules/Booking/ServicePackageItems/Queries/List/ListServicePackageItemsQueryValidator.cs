namespace Detailly.Application.Modules.Booking.ServicePackageItems.Queries.List;

public sealed class ListServicePackageItemsQueryValidator : AbstractValidator<ListServicePackageItemsQuery>
{
    public ListServicePackageItemsQueryValidator()
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
