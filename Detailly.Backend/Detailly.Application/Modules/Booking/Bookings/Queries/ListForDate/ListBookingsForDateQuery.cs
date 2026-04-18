using Detailly.Domain.Common.Enums;

namespace Detailly.Application.Modules.Booking.Bookings.Queries.ListForDate;

public class ListBookingsForDateQuery : BasePagedQuery<ListBookingsForDateQueryDto>
{
    public DateTime DateUtc { get; set; }
    public int ShopLocationId { get; set; }
    public ServiceMode ServiceMode { get; set; }
    public bool IncludePendingPaymentHolds { get; set; } = false;
}