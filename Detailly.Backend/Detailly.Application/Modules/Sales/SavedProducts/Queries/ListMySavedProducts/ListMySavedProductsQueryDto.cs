namespace Detailly.Application.Modules.Sales.SavedProducts.Queries.ListMySavedProducts;

public sealed class ListMySavedProductsQueryDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = default!;
    public decimal Price { get; set; }
    public bool ProductIsEnabled { get; set; }
    public int QuantityInStock { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}