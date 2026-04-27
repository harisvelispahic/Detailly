namespace Detailly.Application.Abstractions.Booking;

public sealed class BookingQuoteResult
{
    public required int ServicePackageId { get; init; }

    /// <summary>Total booking duration. For InShop fleet this is the parallel (single-vehicle) duration.
    /// For mobile fleet the handler overrides this using the k-optimisation result.</summary>
    public required int TotalDurationMinutes { get; init; }

    /// <summary>Duration to service exactly one vehicle — always baseDuration + addonsDuration.</summary>
    public required int PerVehicleDurationMinutes { get; init; }

    public required int RequiredEmployees { get; init; }
    public required int RequiredBays { get; init; }

    public required decimal TotalPrice { get; init; }
    public decimal MobileSurchargeFee { get; init; }

    /// <summary>One-way travel time in minutes from the shop to the service address. 0 for InShop.</summary>
    public int TravelTimeMinutes { get; init; }

    public required List<AddonSnapshot> Addons { get; init; } = new();

    public sealed class AddonSnapshot
    {
        public required int ServicePackageItemId { get; init; }
        public required decimal Price { get; init; }
        public required int DurationMinutes { get; init; }
        public required int RequiredEmployees { get; init; }
    }
}
