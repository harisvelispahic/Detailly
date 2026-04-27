namespace Detailly.Application.Modules.Booking.Bookings.Queries.GetAvailability;

public sealed class GetAvailabilityResult
{
    public List<GetAvailabilityQueryDto> Slots { get; set; } = new();
    public int TravelTimeMinutes { get; set; }
    public decimal MobileSurchargeFee { get; set; }
}
