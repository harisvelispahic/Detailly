using Detailly.Domain.Common;

namespace Detailly.Domain.Entities.Vehicle;

public class VehicleCategoryEntity : BaseEntity
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public decimal BasePriceMultiplier{ get; set; }

    // Foreign keys
    public IReadOnlyCollection<VehicleEntity> Vehicles { get; private set; } = new List<VehicleEntity>();
}
