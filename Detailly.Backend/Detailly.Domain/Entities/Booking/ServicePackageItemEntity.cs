using Detailly.Domain.Common;
using Detailly.Domain.Entities.Shared;

namespace Detailly.Domain.Entities.Booking;

public class ServicePackageItemEntity : BaseEntity
{
    public required string Name { get; set; }
    public required decimal Price { get; set; }
    public string? Description { get; set; }

    // Foreign keys
    public IReadOnlyCollection<ServicePackageItemAssignmentEntity> ServicePackageItemAssignments { get; private set; } = new List<ServicePackageItemAssignmentEntity>();
    public ICollection<ImageEntity> Images { get; set; } = new List<ImageEntity>();

}
