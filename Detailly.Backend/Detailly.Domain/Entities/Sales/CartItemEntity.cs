using Detailly.Domain.Common;
using Detailly.Domain.Entities.Catalog;

namespace Detailly.Domain.Entities.Sales;

public class CartItemEntity : BaseEntity
{
    public required decimal UnitPrice { get; set; }
    public required int Quantity { get; set; }
    public decimal LineTotal { get; set; }


    // Foreign keys
    public required int ProductId { get; set; }
    public ProductEntity Product { get; set; } = null!;
    public  required int CartId { get; set; }
    public CartEntity Cart { get; set; } = null!;
}
