
namespace Detailly.Application.Modules.Booking.Bookings.Commands.Cancel;

public sealed class CancelBookingCommand : IRequest<Unit>
{
    public required int BookingId { get; set; }
    public string? Reason { get; set; }
}