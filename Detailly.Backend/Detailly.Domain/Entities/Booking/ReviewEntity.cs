using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Detailly.Domain.Entities.Booking
{
    public class ReviewEntity
    {
        [Key, ForeignKey(nameof(Booking))]
        public int BookingId { get; set; } // PK and FK
        public int Rating { get; set; }
        public string? Description { get; set; }
        public string? Image { get; set; }
        public int ValueForMoney { get; set; }

        // Foreign keys
        public BookingEntity Booking { get; set; } = null!;
    }
}
