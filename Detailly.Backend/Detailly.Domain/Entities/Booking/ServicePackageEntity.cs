using Detailly.Domain.Common;

namespace Detailly.Domain.Entities.Booking;

public class ServicePackageEntity : BaseEntity
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public required decimal Price { get; set; }

    public int? BaseDurationMinutes { get; set; }
    public int? BaseRequiredEmployees { get; set; }        // required staff for base package

    // Foreign keys
    public IReadOnlyCollection<ServicePackageItemAssignmentEntity> ServicePackageItemAssignments { get; private set; } = new List<ServicePackageItemAssignmentEntity>();
    public IReadOnlyCollection<BookingEntity> Bookings { get; private set; } = new List<BookingEntity>();
    
}
