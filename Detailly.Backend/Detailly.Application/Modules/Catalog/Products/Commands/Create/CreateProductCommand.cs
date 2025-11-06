
using Detailly.Domain.Common.Enums;

namespace Detailly.Application.Modules.Catalog.Products.Commands.Create;

public class CreateProductCommand : IRequest<int>
{
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required decimal Price { get; set; }
    public required int CategoryId { get; set; }
    public CurrencyName? Currency { get; set; } = default(CurrencyName?);
    public string? Tags { get; set; }
    public CreateProductCommandInventory Inventory { get; set; }
    public List<CreateProductCommandImages>? Images { get; set; } = new List<CreateProductCommandImages>();
}

public class CreateProductCommandInventory
{
    public required int QuantityInStock { get; set; }
    public int? ReorderLevel { get; set; }
    public int? ReorderQuantity { get; set; }
}

public class CreateProductCommandImages
{
    public required string ImageUrl { get; set; }
    public required int ProductId { get; set; }
    public string? AltText { get; set; }
    public bool? IsThumbnail { get; set; }
    public int? DisplayOrder { get; set; }
}