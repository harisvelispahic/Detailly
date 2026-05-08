using Detailly.Domain.Common;
using Detailly.Domain.Entities.Identity;
using System.ComponentModel.DataAnnotations;

namespace Detailly.Domain.Entities.Booking;

public class ReviewEntity : BaseEntity
{
    public int BookingId { get; set; }        // most recent booking used for rating
    public int ServicePackageId { get; set; } // one review per customer per service package
    public int CustomerId { get; set; }

    [Range(1, 5)]
    public required int Rating { get; set; }
    public string? Description { get; set; }

    public BookingEntity Booking { get; set; } = null!;
    public ServicePackageEntity ServicePackage { get; set; } = null!;
    public ApplicationUserEntity Customer { get; set; } = null!;
}
