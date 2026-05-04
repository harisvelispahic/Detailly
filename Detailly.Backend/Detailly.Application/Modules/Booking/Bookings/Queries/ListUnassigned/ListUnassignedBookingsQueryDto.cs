using Detailly.Domain.Common.Enums;

namespace Detailly.Application.Modules.Booking.Bookings.Queries.ListUnassigned;

public sealed class ListUnassignedBookingsQueryDto
{
    public int Id { get; set; }
    public DateTime StartUtc { get; set; }
    public DateTime EndUtc { get; set; }
    public int RequiredEmployees { get; set; }
    public ServiceMode ServiceMode { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string ServicePackageName { get; set; } = string.Empty;
    public decimal TotalPrice { get; set; }
    public string? Notes { get; set; }
}
