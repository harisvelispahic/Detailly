namespace Detailly.Application.Modules.Catalog.ProductCategories.Queries.List;

public sealed class ListProductCategoriesQueryValidator : AbstractValidator<ListProductCategoriesQuery>
{
    public ListProductCategoriesQueryValidator()
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
