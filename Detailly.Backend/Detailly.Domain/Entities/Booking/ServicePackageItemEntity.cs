using Detailly.Domain.Common;
using Detailly.Domain.Entities.Catalog;

namespace Detailly.Domain.Entities.Booking
{
    public class ServicePackageItemEntity : BaseEntity
    {
        public string? ItemName { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }

        public IReadOnlyCollection<ServicePackageItemAssignmentEntity> ServicePackageItemAssignments { get; private set; } = new List<ServicePackageItemAssignmentEntity>();
    }
}
