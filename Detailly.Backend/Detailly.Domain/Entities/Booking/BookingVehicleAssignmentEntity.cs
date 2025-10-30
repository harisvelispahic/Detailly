using Detailly.Domain.Entities.Vehicle;

namespace Detailly.Domain.Entities.Booking
{
    public class BookingVehicleAssignmentEntity
    {
        public int BookingId { get; set; }
        public BookingEntity? Booking { get; set; }
        public int VehicleId { get; set; }
        public VehicleEntity? Vehicle { get; set; }
    }
}
