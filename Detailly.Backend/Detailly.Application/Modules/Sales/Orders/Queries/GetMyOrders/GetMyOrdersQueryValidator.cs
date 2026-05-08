namespace Detailly.Application.Modules.Sales.Orders.Queries.GetMyOrders;

public sealed class GetMyOrdersQueryValidator : AbstractValidator<GetMyOrdersQuery>
{
    public GetMyOrdersQueryValidator()
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
