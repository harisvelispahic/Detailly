using Detailly.Domain.Common;
using Detailly.Domain.Common.Enums;
using Detailly.Domain.Entities.Identity;
using Detailly.Domain.Entities.Shared;
using System.ComponentModel.DataAnnotations.Schema;

namespace Detailly.Domain.Entities.Sales;

public class OrderEntity : BaseEntity
{

    [ForeignKey(nameof(ShipToAddressId))]
    public required int ShipToAddressId { get; set; }
    public AddressEntity ShipToAddress { get; set; } = null!;
    public required string OrderNumber { get; set; }
    public required DateTime OrderDate { get; set; }
    public decimal TotalAmount { get; set; }
    public string? Notes { get; set; }

    // Foreign keys
    public required int ApplicationUserId { get; set; }
    public ApplicationUserEntity ApplicationUser { get; set; } = null!;
    public OrderStatus Status { get; set; } = OrderStatus.Draft;
    public IReadOnlyCollection<OrderItemAssignmentEntity> OrderItemAssignments { get; private set; } = new List<OrderItemAssignmentEntity>();
}
