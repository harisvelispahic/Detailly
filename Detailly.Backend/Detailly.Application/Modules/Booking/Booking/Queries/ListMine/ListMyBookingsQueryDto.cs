
using Detailly.Domain.Common.Enums;

namespace Detailly.Application.Modules.Booking.Bookings.Queries.ListMine;

public sealed class ListMyBookingsQueryDto
{
    public required int Id { get; set; }
    public required BookingStatus Status { get; set; }
    public required DateTime StartUtc { get; set; }
    public required DateTime EndUtc { get; set; }
    public required decimal TotalPrice { get; set; }
    public required string ServicePackageName { get; set; }
}