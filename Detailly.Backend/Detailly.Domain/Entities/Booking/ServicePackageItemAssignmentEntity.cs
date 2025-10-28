using Detailly.Domain.Common;

namespace Detailly.Domain.Entities.Booking
{
    public class ServicePackageItemAssignmentEntity : BaseEntity
    {
        // FK na paket
        public int ServicePackageId { get; set; }
        public ServicePackageEntity? ServicePackage { get; set; }

        // FK na item
        public int ServicePackageItemId { get; set; }
        public ServicePackageItemEntity? ServicePackageItem { get; set; }
    }
}
