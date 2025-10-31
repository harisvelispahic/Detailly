using Detailly.Domain.Common;

namespace Detailly.Domain.Entities.Booking
{
    public class ServicePackageEntity : BaseEntity
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int EstimatedDurationHours { get; set; } // In hours, will be converted to Days on FE if needed

        // Foreign keys
        public IReadOnlyCollection<ServicePackageItemAssignmentEntity> ServicePackageItemAssignments { get; private set; } = new List<ServicePackageItemAssignmentEntity>();
        public IReadOnlyCollection<BookingEntity> Bookings { get; private set; } = new List<BookingEntity>();
        
    }
}
