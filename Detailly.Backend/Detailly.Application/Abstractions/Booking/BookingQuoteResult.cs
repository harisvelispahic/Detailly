namespace Detailly.Application.Abstractions.Booking;

public sealed class BookingQuoteResult
{
    public required int ServicePackageId { get; init; }

    public required int TotalDurationMinutes { get; init; }
    public required int RequiredEmployees { get; init; }
    public required int RequiredBays { get; init; }

    public required decimal TotalPrice { get; init; }

    public required List<AddonSnapshot> Addons { get; init; } = new();

    public sealed class AddonSnapshot
    {
        public required int ServicePackageItemId { get; init; }
        public required decimal Price { get; init; }
        public required int DurationMinutes { get; init; }
        public required int RequiredEmployees { get; init; }
    }
}