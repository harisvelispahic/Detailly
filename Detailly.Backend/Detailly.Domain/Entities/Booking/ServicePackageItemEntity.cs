using Detailly.Domain.Common;
using Detailly.Domain.Entities.Shared;

namespace Detailly.Domain.Entities.Booking;

public class ServicePackageItemEntity : BaseEntity
{
    public required string Name { get; set; }
    public required decimal Price { get; set; }
    public string? Description { get; set; }


    // NEW - scheduling drivers
    public required int DurationMinutes { get; set; }
    public required int RequiredEmployees { get; set; } = 1;


    public bool IsAddon { get; set; } = false;
    public bool IsActive { get; set; } = true;


    // Foreign keys
    public ICollection<ImageEntity> Images { get; set; } = new List<ImageEntity>();
    public IReadOnlyCollection<ServicePackageItemAssignmentEntity> ServicePackageItemAssignments { get; private set; } = new List<ServicePackageItemAssignmentEntity>();
    public IReadOnlyCollection<BookingItemEntity> BookingItems { get; private set; } = new List<BookingItemEntity>();
}
