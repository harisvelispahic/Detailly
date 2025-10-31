using Detailly.Domain.Common;

namespace Detailly.Domain.Entities.Booking
{
    public class TimeSlotEntity : BaseEntity
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool IsAvailable { get; set; }
        public string? Notes { get; set; }

        // Foreign keys
        public int LocationId { get; set; }
        public LocationEntity Location { get; set; } = null!;
        
        public IReadOnlyCollection<BookingEntity> Bookings { get; private set; } = new List<BookingEntity>();

    }
}
