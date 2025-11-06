using Detailly.Domain.Common;
using Detailly.Domain.Entities.Vehicle;

namespace Detailly.Domain.Entities.Booking;

public class BookingVehicleAssignmentEntity : BaseEntity
{

    // Foreign keys
    public required int BookingId { get; set; }
    public BookingEntity Booking { get; set; } = null!;
    public required int VehicleId { get; set; }
    public VehicleEntity Vehicle { get; set; } = null!;
}
