using Detailly.Domain.Entities.Booking;
using Detailly.Domain.Entities.Identity;

namespace Detailly.Domain.Entities.Vehicle
{
    public class VehicleEntity
    {
        public string? Model { get; set; }
        public string? Brand { get; set; }
        public int YearOfManufacture { get; set; }
        public string? Notes { get; set; }
        public int ApplicationUserId { get; set; }
        public ApplicationUserEntity? ApplicationUser { get; set; }


        public IReadOnlyCollection<BookingVehicleAssignmentEntity> BookingVehicleAssignments { get; private set; } = new List<BookingVehicleAssignmentEntity>();

    }
}
