using Detailly.Domain.Common.Enums;

namespace Detailly.Application.Modules.Booking.Bookings.Queries.ListMyAssigned;

public sealed class ListMyAssignedBookingsQueryDto
{
    public int Id { get; set; }
    public BookingStatus Status { get; set; }
    public ServiceMode ServiceMode { get; set; }
    public DateTime StartUtc { get; set; }
    public DateTime EndUtc { get; set; }
    public decimal TotalPrice { get; set; }
    public string ServicePackageName { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string ShopLocationName { get; set; } = string.Empty;
    public string? Notes { get; set; }
}
