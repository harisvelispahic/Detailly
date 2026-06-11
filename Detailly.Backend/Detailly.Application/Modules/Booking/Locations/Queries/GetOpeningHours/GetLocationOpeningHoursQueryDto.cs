namespace Detailly.Application.Modules.Booking.Locations.Queries.GetOpeningHours;

public sealed class GetLocationOpeningHoursQueryDto
{
    public required int DayOfWeek { get; init; }  // 0=Sunday ... 6=Saturday
    public required bool IsClosed { get; init; }
    public int? OpenHour { get; init; }    // 0–23 local (Bosnia time)
    public int? OpenMinute { get; init; }  // 0–59
    public int? CloseHour { get; init; }
    public int? CloseMinute { get; init; }
}
