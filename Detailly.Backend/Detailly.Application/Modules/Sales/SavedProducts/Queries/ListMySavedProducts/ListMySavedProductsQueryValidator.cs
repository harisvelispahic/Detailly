namespace Detailly.Application.Modules.Sales.SavedProducts.Queries.ListMySavedProducts;

public sealed class ListMySavedProductsQueryValidator : AbstractValidator<ListMySavedProductsQuery>
{
    public ListMySavedProductsQueryValidator()
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
