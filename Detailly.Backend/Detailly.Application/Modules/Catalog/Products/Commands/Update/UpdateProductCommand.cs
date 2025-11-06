
namespace Detailly.Application.Modules.Catalog.Products.Commands.Update;

public sealed class UpdateProductCommand : IRequest<Unit>
{
    [JsonIgnore]
    public required int Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal? Price { get; set; }
    public string? Tags { get; set; } // comma-separated tags
    public UpdateProductCommandInventory Inventory { get; set; }
}

public class UpdateProductCommandInventory
{
    public int? QuantityInStock { get; set; }
    public int? ReorderLevel { get; set; }
    public int? ReorderQuantity { get; set; }
}