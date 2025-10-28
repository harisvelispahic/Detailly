using Detailly.Domain.Common;

namespace Detailly.Domain.Entities.Booking
{
    public class BookingEntity : BaseEntity
    {
        public int ApplicationUserId { get; set; }
        public int ServicePackageId { get; set; }
        public ServicePackageEntity? ServicePackage { get; set; }
        public int TimeSlotId { get; set; }
        public TimeSlotEntity? TimeSlot { get; set; }
        public decimal TotalPrice { get; set; }
        public string? Status { get; set; }
        public string? Notes { get; set; }


        // Navigacija ka Review
        public ReviewEntity? Review { get; set; }
    }
}
