using Detailly.Domain.Common;
using Detailly.Domain.Entities.Identity;
using Detailly.Domain.Entities.Shared;
using System.ComponentModel.DataAnnotations.Schema;

namespace Detailly.Domain.Entities.Sales
{
    public class OrderEntity : BaseEntity
    {
        
        [ForeignKey(nameof(ShipToAddressId))]
        public int ShipToAddressId { get; set; }
        public AddressEntity ShipToAddress { get; set; } = null!;
        public string? OrderName { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string? Notes { get; set; }

        // Foreign keys
        public int ApplicationUserId { get; set; }
        public ApplicationUserEntity ApplicationUser { get; set; } = null!;
        public int OrderStatusId { get; set; }
        public OrderStatusEntity OrderStatus { get; set; } = null!;
        
        public IReadOnlyCollection<OrderItemAssignmentEntity> OrderItemAssignments { get; private set; } = new List<OrderItemAssignmentEntity>();
    }
}
