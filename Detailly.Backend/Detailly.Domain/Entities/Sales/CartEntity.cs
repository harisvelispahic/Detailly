using Detailly.Domain.Common;
using Detailly.Domain.Common.Enums;
using Detailly.Domain.Entities.Identity;

namespace Detailly.Domain.Entities.Sales;

public class CartEntity : BaseEntity
{
    public bool IsEmpty { get; set; }
    public decimal TotalAmount { get; set; }
    public string? Notes { get; set; }
    public CartStatus Status { get; set; } = CartStatus.Active; 

    // Foreign keys
    public required int ApplicationUserId { get; set; }
    public ApplicationUserEntity ApplicationUser { get; set; } = null!;
    
    public IReadOnlyCollection<CartItemEntity> CartItems { get; private set; } = new List<CartItemEntity>();
}
