using Detailly.Domain.Common;

namespace Detailly.Domain.Entities.Booking
{
    public class ServicePackageItemAssignmentEntity : BaseEntity
    {


        // Foreign keys
        public int ServicePackageId { get; set; }
        public ServicePackageEntity ServicePackage { get; set; } = null!;
        public int ServicePackageItemId { get; set; }
        public ServicePackageItemEntity ServicePackageItem { get; set; } = null!;
    }
}
