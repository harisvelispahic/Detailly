using Detailly.Domain.Entities.Booking;

namespace Detailly.Domain.Entities.Payment
{
    public class WalletEntity
    {
        public decimal Balance { get; set; }
        public string? Currency { get; set; }   // prebaciti u enum
        public decimal TotalDeposited { get; set; }
        public int PercentageAdded { get; set; }


        public IReadOnlyCollection<BookingVehicleAssignmentEntity> BookingVehicleAssignments { get; private set; } = new List<BookingVehicleAssignmentEntity>();

    }
}
