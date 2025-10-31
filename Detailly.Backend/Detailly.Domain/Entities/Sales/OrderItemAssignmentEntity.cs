using Detailly.Domain.Common;

namespace Detailly.Domain.Entities.Sales
{
    public class OrderItemAssignmentEntity : BaseEntity
    {
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineTotal { get; set; }

        // Foreign keys
        public int OrderId { get; set; }
        public OrderEntity Order { get; set; } = null!;
        public int OrderItemId {  get; set; }
        public OrderItemEntity OrderItem { get; set; } = null!;
    }
}
