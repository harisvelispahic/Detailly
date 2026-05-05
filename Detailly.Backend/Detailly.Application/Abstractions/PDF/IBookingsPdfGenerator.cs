using Detailly.Application.Modules.Booking.Bookings.Queries.ExportMyBookings;

namespace Detailly.Application.Abstractions.PDF;

public interface IBookingsPdfGenerator
{
    byte[] Generate(List<ExportMyBookingsQueryDto> bookings, DateTime startDate, DateTime endDate, string customerName);
}
