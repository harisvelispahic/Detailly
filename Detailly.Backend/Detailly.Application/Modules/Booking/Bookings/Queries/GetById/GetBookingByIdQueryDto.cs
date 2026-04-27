using Detailly.Domain.Common.Enums;

namespace Detailly.Application.Modules.Booking.Bookings.Queries.GetById;

public sealed class GetBookingByIdQueryDto
{
    public required int Id { get; set; }
    public required BookingStatus Status { get; set; }
    public required ServiceMode ServiceMode { get; set; }

    public required DateTime StartUtc { get; set; }
    public required DateTime EndUtc { get; set; }

    public required decimal TotalPrice { get; set; }
    public decimal? MobileSurchargeFee { get; set; }
    public required int RequiredEmployees { get; set; }
    public required int RequiredBays { get; set; }
    public int? TravelTimeMinutes { get; set; }
    public DateTime? DepartureUtc { get; set; }
    public DateTime? ReturnUtc { get; set; }

    public DateTime? ReservationExpiresAtUtc { get; set; }
    public string? Notes { get; set; }

    public required int ServicePackageId { get; set; }
    public required string ServicePackageName { get; set; }

    public int? PaymentTransactionId { get; set; }
    public PaymentTransactionStatus? PaymentStatus { get; set; }

    public List<BookingAddonDto> Addons { get; set; } = new();
    public List<int> VehicleIds { get; set; } = new();
}
public sealed class BookingAddonDto
{
    public required int BookingItemId { get; set; }
    public required string Name { get; set; }
    public required decimal PriceSnapshot { get; set; }
    public required int DurationMinutesSnapshot { get; set; }
    public required int RequiredEmployeesSnapshot { get; set; }
}