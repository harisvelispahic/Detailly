using Detailly.Domain.Common;
using Detailly.Domain.Entities.Identity;

namespace Detailly.Domain.Entities.Booking;

public class BookingEmployeeAssignmentEntity : BaseEntity
{
    public required int BookingId { get; set; }
    public BookingEntity Booking { get; set; } = null!;

    public required int EmployeeId { get; set; }
    public ApplicationUserEntity Employee { get; set; } = null!;

    public DateTime AssignedAtUtc { get; set; } = DateTime.UtcNow;
    public bool IsLead { get; set; } = false;
}