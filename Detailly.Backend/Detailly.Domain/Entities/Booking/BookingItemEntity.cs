
using Detailly.Domain.Common;

namespace Detailly.Domain.Entities.Booking;

public class BookingItemEntity : BaseEntity
{
    public required int BookingId { get; set; }
    public BookingEntity Booking { get; set; } = null!;


    public required int ServicePackageItemId { get; set; }
    public ServicePackageItemEntity ServicePackageItem { get; set; } = null!;


    // Snapshot fields (history-safe)
    public required decimal PriceSnapshot { get; set; }
    public required int DurationMinutesSnapshot { get; set; }
    public required int RequiredEmployeesSnapshot { get; set; }


    // True if the item is not part of the base package and was selected as an add-on
    public bool IsAddon { get; set; } = true;
}