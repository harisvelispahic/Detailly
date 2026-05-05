namespace Detailly.Application.Modules.Booking.Bookings.Commands.ExportMyBookingsPdf;

public sealed class ExportMyBookingsPdfCommand : IRequest<byte[]>
{
    public DateTime StartDateUtc { get; set; }
    public DateTime EndDateUtc { get; set; }
    public string CustomerName { get; set; } = string.Empty;
}
