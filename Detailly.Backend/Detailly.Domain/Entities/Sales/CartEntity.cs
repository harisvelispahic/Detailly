using Detailly.Domain.Common;
using Detailly.Domain.Entities.Identity;

namespace Detailly.Domain.Entities.Sales
{
    public class CartEntity : BaseEntity
    {
        public bool IsEmpty { get; set; }
        public decimal TotalAmount { get; set; }
        public string? Notes { get; set; }
        public string? Status { get; set; }     // implementirati enum

        // Foreign keys
        public int? ApplicationUserId { get; set; } // oicionalna 0,1 veza sa ApplicationUser-om
        public ApplicationUserEntity ApplicationUser { get; set; } = null!;
        
        public IReadOnlyCollection<CartItemEntity> CartItems { get; private set; } = new List<CartItemEntity>();
    }
}
