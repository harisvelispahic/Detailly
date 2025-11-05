using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Detailly.Domain.Entities.Catalog;

public class InventoryEntity
{
    [Key, ForeignKey(nameof(Product))]
    public int ProductId { get; set; } // PK and FK
    public required int QuantityInStock { get; set; }
    public int ReorderLevel { get; set; }
    public int ReorderQuantity { get; set; }


    // Foreign keys
    public ProductEntity Product { get; set; } = null!;

    // Base entity
    public bool IsDeleted { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? ModifiedAtUtc { get; set; }

}
