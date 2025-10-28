using Detailly.Domain.Common;

namespace Detailly.Domain.Entities.Booking
{
    public class LocationEntity : BaseEntity
    {
        public int AddressId { get; set; }

        public IReadOnlyCollection<TimeSlotEntity> TimeSlots { get; private set; } = new List<TimeSlotEntity>();
    }
}
