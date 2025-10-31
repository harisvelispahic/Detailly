using Detailly.Domain.Common;

namespace Detailly.Domain.Entities.Booking
{
    public class ServicePackageItemEntity : BaseEntity
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }

        // Foreign keys
        public IReadOnlyCollection<ServicePackageItemAssignmentEntity> ServicePackageItemAssignments { get; private set; } = new List<ServicePackageItemAssignmentEntity>();
    }
}
