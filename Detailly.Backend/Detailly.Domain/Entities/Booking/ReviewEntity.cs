using Detailly.Domain.Entities.Shared;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Detailly.Domain.Entities.Booking;

public class ReviewEntity
{
    [Key, ForeignKey(nameof(Booking))]
    public int BookingId { get; set; } // PK and FK
    [Range(1, 5)]
    public required int Rating { get; set; }
    public string? Description { get; set; }
    public string? Image { get; set; }
    public int? ValueForMoney { get; set; }

    // Foreign keys
    public BookingEntity Booking { get; set; } = null!;
    public ICollection<ImageEntity> Images { get; set; } = new List<ImageEntity>();


    // Base entity
    public bool IsDeleted { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? ModifiedAtUtc { get; set; }
}
