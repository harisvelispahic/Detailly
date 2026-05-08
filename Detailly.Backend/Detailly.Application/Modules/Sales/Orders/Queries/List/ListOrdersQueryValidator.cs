namespace Detailly.Application.Modules.Sales.Orders.Queries.List;

public sealed class ListOrdersQueryValidator : AbstractValidator<ListOrdersQuery>
{
    public ListOrdersQueryValidator()
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
