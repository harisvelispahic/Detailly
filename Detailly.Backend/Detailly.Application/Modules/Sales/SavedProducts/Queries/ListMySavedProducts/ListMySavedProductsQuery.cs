namespace Detailly.Application.Modules.Sales.SavedProducts.Queries.ListMySavedProducts;

public sealed class ListMySavedProductsQuery : BasePagedQuery<ListMySavedProductsQueryDto>
{
    public string? Search { get; init; }
}