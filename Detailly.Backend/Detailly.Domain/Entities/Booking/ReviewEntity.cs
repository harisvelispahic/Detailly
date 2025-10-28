using Detailly.Domain.Common;

namespace Detailly.Domain.Entities.Booking
{
    public class ReviewEntity : BaseEntity
    {
        public int BookingId { get; set; }
        public BookingEntity? Booking { get; set; }
        public int Rating { get; set; }
        public string? Description { get; set; }
        public string? Image { get; set; }
        public int ValueForMoney { get; set; }
    }
}
