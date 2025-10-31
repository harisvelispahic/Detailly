using Detailly.Domain.Common;
using Detailly.Domain.Entities.Catalog;

namespace Detailly.Domain.Entities.Sales
{
    public class OrderItemEntity : BaseEntity
    {
        public int UnitPrice { get; set; }

        // Foreign keys
        public int ProductId { get; set; }
        public ProductEntity Product { get; set; } = null!;
        public IReadOnlyCollection<OrderItemAssignmentEntity> OrderItemAssignments { get; private set; } = new List<OrderItemAssignmentEntity>();
    }
}
