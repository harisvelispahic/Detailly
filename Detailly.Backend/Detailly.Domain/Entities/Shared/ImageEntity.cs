using Detailly.Domain.Common;
using Detailly.Domain.Entities.Booking;
using Detailly.Domain.Entities.Catalog;
using Detailly.Domain.Entities.Identity;

namespace Detailly.Domain.Entities.Shared;

public class ImageEntity : BaseEntity
{
    public required string ImageUrl { get; set; }
    public string? AltText { get; set; }
    public bool IsThumbnail { get; set; }
    public int DisplayOrder { get; set; }

    // Relationships
    public int? ProductId { get; set; }
    public ProductEntity? Product { get; set; }

    public int? ReviewId { get; set; }
    public ReviewEntity? Review { get; set; }

    public int? ApplicationUserId { get; set; }
    public ApplicationUserEntity? ApplicationUser { get; set; }

    public int? ServicePackageItemId { get; set; }
    public ServicePackageItemEntity? ServicePackageItem { get; set; }
}
