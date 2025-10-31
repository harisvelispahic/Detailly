using Detailly.Domain.Common;
using Detailly.Domain.Entities.Catalog;

namespace Detailly.Domain.Entities.Sales
{
    public class CartItemEntity : BaseEntity
    {
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }

        // Foreign keys
        public int ProductId { get; set; }
        public ProductEntity Product { get; set; } = null!;
        public int CartId { get; set; }
        public CartEntity Cart { get; set; } = null!;
    }
}
