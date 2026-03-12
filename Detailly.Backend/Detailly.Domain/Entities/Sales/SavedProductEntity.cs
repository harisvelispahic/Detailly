using Detailly.Domain.Common;
using Detailly.Domain.Entities.Catalog;
using Detailly.Domain.Entities.Identity;

namespace Detailly.Domain.Entities.Sales;

public class SavedProductEntity : BaseEntity
{
    public int ApplicationUserId { get; set; }
    public ApplicationUserEntity ApplicationUser { get; set; } = null!;

    public int ProductId { get; set; }
    public ProductEntity Product { get; set; } = null!;
}