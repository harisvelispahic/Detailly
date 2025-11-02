using Detailly.Domain.Common;
using Detailly.Domain.Entities.Vehicle;

namespace Detailly.Domain.Entities.Booking
{
    public class BookingVehicleAssignmentEntity : BaseEntity
    {

        // Foreign keys
        public int BookingId { get; set; }
        public BookingEntity Booking { get; set; } = null!;
        public int VehicleId { get; set; }
        public VehicleEntity Vehicle { get; set; } = null!;
    }
}
