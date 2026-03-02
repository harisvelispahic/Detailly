
namespace Detailly.Application.Modules.Booking.Bookings.Commands.Complete;

public sealed class CompleteBookingCommand : IRequest<Unit>
{
    public required int BookingId { get; set; }
}