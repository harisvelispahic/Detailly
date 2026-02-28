
using Detailly.Domain.Common;

namespace Detailly.Domain.Entities.Booking;

public sealed class LocationOpeningHoursEntity : BaseEntity
{
    public required int ShopLocationId { get; set; }
    public LocationEntity ShopLocation { get; set; } = null!;

    // 0=Sunday ... 6=Saturday (System.DayOfWeek)
    public required int DayOfWeek { get; set; }

    public bool IsClosed { get; set; } = false;

    // "time of day" in UTC
    public TimeSpan? OpenTimeUtc { get; set; }
    public TimeSpan? CloseTimeUtc { get; set; }
}