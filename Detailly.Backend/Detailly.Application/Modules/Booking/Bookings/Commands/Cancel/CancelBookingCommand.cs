namespace Detailly.Application.Modules.Booking.Bookings.Commands.Cancel;

public sealed class CancelBookingCommand : IRequest<Unit>
{
    [JsonIgnore]
    public int BookingId { get; set; }
    public string? Reason { get; set; }
}