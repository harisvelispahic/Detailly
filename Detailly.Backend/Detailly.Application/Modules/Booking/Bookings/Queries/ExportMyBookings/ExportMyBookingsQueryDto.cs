using Detailly.Domain.Common.Enums;

namespace Detailly.Application.Modules.Booking.Bookings.Queries.ExportMyBookings;

public class ExportMyBookingsQueryDto
{
    public int Id { get; set; }
    public BookingStatus Status { get; set; }
    public DateTime StartUtc { get; set; }
    public DateTime EndUtc { get; set; }
    public decimal TotalPrice { get; set; }
    public string ServicePackageName { get; set; } = string.Empty;
}
