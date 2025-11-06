using Detailly.Domain.Common;

namespace Detailly.Domain.Entities.Sales;

public class OrderItemAssignmentEntity : BaseEntity
{
    public required int Quantity { get; set; }
    public required decimal UnitPrice { get; set; }
    public decimal LineTotal { get; set; }

    // Foreign keys
    public required int OrderId { get; set; }
    public OrderEntity Order { get; set; } = null!;
    public required int OrderItemId { get; set; }
    public OrderItemEntity OrderItem { get; set; } = null!;
}
