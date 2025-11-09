using Detailly.Domain.Common;
using Detailly.Domain.Common.Enums;
using Detailly.Domain.Entities.Catalog;

namespace Detailly.Domain.Entities.Sales;

public class OrderItemEntity : BaseEntity
{
    public required decimal UnitPrice { get; set; }
    public required CurrencyName Currency { get; set; } = CurrencyName.BAM;
    public required int Quantity { get; set; }
    public decimal LineSubtotal { get; set; }
    public decimal DiscountPercentage { get; set; } = 0.05m;
    public decimal LineTotal { get; set; }

    // Foreign keys
    public required int OrderId { get; set; }
    public OrderEntity Order { get; set; } = null!;
    public required int ProductId { get; set; }
    public ProductEntity Product { get; set; } = null!;
}
