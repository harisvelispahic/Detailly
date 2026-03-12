namespace Detailly.Application.Modules.Sales.SavedProducts.Queries.GetMySavedProducts;

public sealed class GetMySavedProductsQueryDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = default!;
    public decimal Price { get; set; }
    public bool ProductIsEnabled { get; set; }
    public int QuantityInStock { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}