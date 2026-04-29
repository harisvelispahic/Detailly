using Detailly.Domain.Common;
using Detailly.Domain.Entities.Shared;

namespace Detailly.Domain.Entities.Booking;

public class LocationEntity : BaseEntity
{
    public required string Name { get; set; }
    public string? Description { get; set; }

    public required int TotalBays { get; set; }

    public bool IsTemporarilyClosed { get; set; } = false;

    // Foreign keys
    public required int AddressId { get; set; }
    public AddressEntity Address { get; set; } = null!;

    public IReadOnlyCollection<LocationOpeningHoursEntity> LocationOpeningHours { get; private set; }
        = new List<LocationOpeningHoursEntity>();
}
