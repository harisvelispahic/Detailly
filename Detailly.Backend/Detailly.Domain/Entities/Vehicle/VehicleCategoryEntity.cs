namespace Detailly.Domain.Entities.Vehicle
{
    public class VehicleCategoryEntity
    {
        public int VehicleId { get; set; }
        public VehicleEntity? Vehicle { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal BasePriceMultiplier{ get; set; }


        public IReadOnlyCollection<VehicleEntity> Vehicles { get; private set; } = new List<VehicleEntity>();
    }
}
