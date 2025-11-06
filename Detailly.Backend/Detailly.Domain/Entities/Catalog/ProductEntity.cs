using Detailly.Domain.Common;
using Detailly.Domain.Common.Enums;
using Detailly.Domain.Entities.Sales;
using Detailly.Domain.Entities.Shared;
using System.ComponentModel.DataAnnotations;

namespace Detailly.Domain.Entities.Catalog;

public class ProductEntity : BaseEntity
{
    [MaxLength(Constraints.NameMaxLength)]
    public required string Name { get; set; }

    [MaxLength(Constraints.DescriptionMaxLength)]
    public required string Description { get; set; }
    public required string ProductNumber { get; set; }
    public required decimal Price { get; set; }
    public required CurrencyName Currency { get; set; } = CurrencyName.BAM;

    //public decimal Weight { get; set; }
    //public decimal Volume { get; set; }
    //public string? UnitOfMeasure { get; set; }  // moze biti enum u buducnosti
    // image
    // thumbnail
    public string? Tags { get; set; } // comma-separated tags
    public bool IsEnabled { get; set; }


    // Foreign keys
    public required int CategoryId { get; set; }
    public ProductCategoryEntity Category { get; set; } = null!;
    public InventoryEntity Inventory { get; set; } = null!;

    public IReadOnlyCollection<OrderItemEntity> OrderItems { get; private set; } = new List<OrderItemEntity>();
    public IReadOnlyCollection<CartItemEntity> CartItems { get; private set; } = new List<CartItemEntity>();
    public ICollection<ImageEntity> Images { get; set; } = new List<ImageEntity>();



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