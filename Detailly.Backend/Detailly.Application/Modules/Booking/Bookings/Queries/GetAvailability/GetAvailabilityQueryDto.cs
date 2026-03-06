namespace Detailly.Application.Modules.Booking.Bookings.Queries.GetAvailability;

public sealed class GetAvailabilityQueryDto
{
    public required DateTime StartUtc { get; set; }
    public required DateTime EndUtc { get; set; }
}