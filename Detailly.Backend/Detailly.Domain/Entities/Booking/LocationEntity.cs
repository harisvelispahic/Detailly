using Detailly.Domain.Common;
using Detailly.Domain.Common.Enums;
using Detailly.Domain.Entities.Shared;

namespace Detailly.Domain.Entities.Booking;

public class LocationEntity : BaseEntity
{
    public required string Name { get; set; }
    public string? Description { get; set; }

    public required LocationType LocationType { get; set; }

    // Capacity (used when ServiceMode = InShop)
    public required int TotalBays { get; set; }

    // Foreign keys
    public required int AddressId { get; set; }
    public AddressEntity Address { get; set; } = null!;
}
