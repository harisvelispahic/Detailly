using Detailly.Domain.Common;
using Detailly.Domain.Entities.Booking;
using Detailly.Domain.Entities.Identity;

namespace Detailly.Domain.Entities.Vehicle
{
    public class VehicleEntity : BaseEntity
    {
        public string? Model { get; set; }
        public string? Brand { get; set; }
        public int YearOfManufacture { get; set; }
        public string? Notes { get; set; }

        
        // Foreign keys
        public int ApplicationUserId { get; set; }
        public ApplicationUserEntity ApplicationUser { get; set; } = null!;
        public int VehicleCategoryId { get; set; }
        public VehicleCategoryEntity VehicleCategory { get; set; } = null!;

        public IReadOnlyCollection<BookingVehicleAssignmentEntity> BookingVehicleAssignments { get; private set; } = new List<BookingVehicleAssignmentEntity>();

    }
}
