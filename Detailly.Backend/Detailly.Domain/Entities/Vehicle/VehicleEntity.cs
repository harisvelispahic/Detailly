using Detailly.Domain.Common;
using Detailly.Domain.Entities.Booking;
using Detailly.Domain.Entities.Identity;

namespace Detailly.Domain.Entities.Vehicle;

public class VehicleEntity : BaseEntity
{
    public required string Brand { get; set; }
    public required string Model { get; set; }
    public required int YearOfManufacture { get; set; }
    public required string LicencePlate { get; set; }
    public string? Notes { get; set; }


    // Foreign keys
    public required int ApplicationUserId { get; set; }
    public ApplicationUserEntity ApplicationUser { get; set; } = null!;
    public required int VehicleCategoryId { get; set; }
    public VehicleCategoryEntity VehicleCategory { get; set; } = null!;

    public IReadOnlyCollection<BookingVehicleAssignmentEntity> BookingVehicleAssignments { get; private set; } = new List<BookingVehicleAssignmentEntity>();

}
