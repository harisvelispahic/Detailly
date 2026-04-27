using System.ComponentModel.DataAnnotations;

namespace Detailly.Shared.Options;

public sealed class FleetDiscountOptions
{
    public const string SectionName = "FleetDiscount";

    /// <summary>Discount percentage applied for a single-vehicle fleet booking.</summary>
    [Range(0, 100)]
    public decimal BaseDiscountPercent { get; init; } = 2.0m;

    /// <summary>Additional discount percentage added for each vehicle beyond the first.</summary>
    [Range(0, 100)]
    public decimal PerVehicleDiscountPercent { get; init; } = 1.0m;

    /// <summary>Maximum discount percentage a fleet booking can receive, regardless of vehicle count.</summary>
    [Range(0, 100)]
    public decimal MaxDiscountPercent { get; init; } = 8.0m;
}
