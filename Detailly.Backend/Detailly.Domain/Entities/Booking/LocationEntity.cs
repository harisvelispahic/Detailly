using Detailly.Domain.Common;
using Detailly.Domain.Entities.Shared;

namespace Detailly.Domain.Entities.Booking
{
    public class LocationEntity : BaseEntity
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        
        // Foreign keys
        public int AddressId { get; set; }
        public AddressEntity Address { get; set; } = null!;
        
        public IReadOnlyCollection<TimeSlotEntity> TimeSlots { get; private set; } = new List<TimeSlotEntity>();
    }
}
