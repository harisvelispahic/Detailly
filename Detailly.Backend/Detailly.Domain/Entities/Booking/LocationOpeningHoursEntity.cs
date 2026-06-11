using Detailly.Domain.Common;

namespace Detailly.Domain.Entities.Booking;

public sealed class LocationOpeningHoursEntity : BaseEntity
{
    public static readonly TimeSpan DefaultOpenTime  = new(8,  0, 0);
    public static readonly TimeSpan DefaultCloseTime = new(20, 0, 0);

    // All locations are in Bosnia (CET/CEST). Used to convert UTC shift times
    // to local time before comparing against stored local opening hours.
    public static readonly TimeZoneInfo LocalZone =
        TimeZoneInfo.FindSystemTimeZoneById("Europe/Sarajevo");

    public required int ShopLocationId { get; set; }
    public LocationEntity ShopLocation { get; set; } = null!;

    // 0=Sunday ... 6=Saturday (System.DayOfWeek)
    public required int DayOfWeek { get; set; }

    public bool IsClosed { get; set; } = false;

    // Local time-of-day (e.g. 08:00 = 8 am Bosnia time, regardless of DST (Daylight Saving Time))
    public TimeSpan? OpenTime { get; set; }
    public TimeSpan? CloseTime { get; set; }
}