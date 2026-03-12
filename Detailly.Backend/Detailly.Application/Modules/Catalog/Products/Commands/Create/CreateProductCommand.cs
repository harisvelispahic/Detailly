using Detailly.Domain.Common.Enums;

namespace Detailly.Application.Modules.Catalog.Products.Commands.Create;

public sealed class CreateProductCommand : IRequest<int>
{
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required decimal Price { get; set; }
    public required int CategoryId { get; set; }
    public CurrencyName? Currency { get; set; }
    public string? Tags { get; set; }

    // Optional now
    public CreateProductCommandInventory? Inventory { get; set; }

    public List<CreateProductCommandImage>? Images { get; set; } = new();
}

public sealed class CreateProductCommandInventory
{
    public int? QuantityInStock { get; set; }
    public int? ReorderLevel { get; set; }
    public int? ReorderQuantity { get; set; }
}

public sealed class CreateProductCommandImage
{
    public required string ImageUrl { get; set; }
    public string? AltText { get; set; }
    public bool? IsThumbnail { get; set; }
    public int? DisplayOrder { get; set; }
}