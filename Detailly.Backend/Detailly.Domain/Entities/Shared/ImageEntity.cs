using Detailly.Domain.Common;
using Detailly.Domain.Entities.Booking;
using Detailly.Domain.Entities.Catalog;

namespace Detailly.Domain.Entities.Shared;

public class ImageEntity : BaseEntity
{
    public required string ImageUrl { get; set; }
    public string? PublicId { get; set; }   // Cloudinary asset identifier (needed for deletion/transforms)
    public string? AltText { get; set; }
    public bool IsThumbnail { get; set; }
    public int DisplayOrder { get; set; }

    // Relationships
    public int? ProductId { get; set; }
    public ProductEntity? Product { get; set; }

    public int? ServicePackageId { get; set; }
    public ServicePackageEntity? ServicePackage { get; set; }
}
