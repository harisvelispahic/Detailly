using System.ComponentModel.DataAnnotations;

namespace Detailly.Shared.Options;

public sealed class OpenRouteServiceOptions
{
    public const string SectionName = "OpenRouteService";

    public string ApiKey { get; init; } = string.Empty;

    [Range(0, double.MaxValue)]
    public decimal FreeRadiusKm { get; init; } = 10.0m;

    [Range(0, double.MaxValue)]
    public decimal PricePerKm { get; init; } = 0.50m;

    [Range(0, double.MaxValue)]
    public decimal FallbackSurcharge { get; init; } = 5.00m;

    [Range(0, int.MaxValue)]
    public int FallbackTravelTimeMinutes { get; init; } = 45;
}
