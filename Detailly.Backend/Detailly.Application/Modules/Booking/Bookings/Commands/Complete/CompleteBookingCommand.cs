namespace Detailly.Application.Modules.Booking.Bookings.Commands.Complete;

public sealed class CompleteBookingCommand : IRequest<Unit>
{
    [JsonIgnore]
    public int BookingId { get; set; }
}