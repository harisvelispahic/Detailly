namespace Detailly.Application.Modules.Booking.Bookings.Queries.ExportMyBookings;

public class ExportMyBookingsQuery : IRequest<List<ExportMyBookingsQueryDto>>
{
    public DateTime StartDateUtc { get; set; }
    public DateTime EndDateUtc { get; set; }
}
