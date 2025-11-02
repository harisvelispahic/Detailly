using Detailly.Domain.Common;
using Detailly.Domain.Entities.Sales;

namespace Detailly.Domain.Entities.Catalog;

public class ProductEntity : BaseEntity
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? ProductNumber { get; set; }
    public decimal Price { get; set; }
    public decimal Weight { get; set; }
    public decimal Volume { get; set; }
    public string? UnitOfMeasure { get; set; }  // moze biti enum u buducnosti
    // image
    // thumbnail
    public string? Tags { get; set; } // comma-separated tags
    public bool IsEnabled { get; set; }


    // Foreign keys
    public int CategoryId { get; set; }
    public ProductCategoryEntity Category { get; set; } = null!;
    public InventoryEntity Inventory { get; set; } = null!;

    public IReadOnlyCollection<OrderItemEntity> OrderItems { get; private set; } = new List<OrderItemEntity>();
    public IReadOnlyCollection<CartItemEntity> CartItems { get; private set; } = new List<CartItemEntity>();



    /// <summary>
    /// Single source of truth for technical/business constraints.
    /// Used in validators and EF configuration.
    /// </summary>
    public static class Constraints
    {
        public const int NameMaxLength = 150;

        public const int DescriptionMaxLength = 1000;
    }
}