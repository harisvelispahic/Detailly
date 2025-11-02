using Detailly.Domain.Common;
using Detailly.Domain.Entities.Identity;

namespace Detailly.Domain.Entities.Booking
{
    public class BookingEntity : BaseEntity
    {
        public decimal TotalPrice { get; set; }
        public string? Status { get; set; }
        public string? Notes { get; set; }




        // Foreign keys
        public int ApplicationUserId { get; set; }
        public ApplicationUserEntity ApplicationUser { get; set; } = null!;
        public int ServicePackageId { get; set; }
        public ServicePackageEntity ServicePackage { get; set; } = null!;
        public int TimeSlotId { get; set; }
        public TimeSlotEntity? TimeSlot { get; set; }
        public ReviewEntity? Review { get; set; }

        public IReadOnlyCollection<BookingVehicleAssignmentEntity> BookingVehicleAssignments { get; private set; } = new List<BookingVehicleAssignmentEntity>();

    }
}
