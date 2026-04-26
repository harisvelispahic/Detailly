using System.Globalization;
using System.Text.Json;
using Detailly.Application.Abstractions.Booking;
using Detailly.Shared.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Detailly.Infrastructure.Booking;

/// <summary>
/// Calculates road distance between two coordinates using the OpenRouteService Directions API.
/// ORS uses GeoJSON coordinate order: longitude first, latitude second.
/// Parameters arrive as (lat, lng) but are passed to ORS as "lng,lat".
/// </summary>
public sealed class OrsRoadDistanceService(
    HttpClient httpClient,
    IOptions<OpenRouteServiceOptions> options,
    ILogger<OrsRoadDistanceService> logger) : IRoadDistanceService
{
    private const string DirectionsBaseUrl = "https://api.openrouteservice.org/v2/directions/driving-car";

    public async Task<decimal?> GetRoadDistanceKmAsync(
        decimal fromLat, decimal fromLng,
        decimal toLat, decimal toLng,
        CancellationToken ct)
    {
        var apiKey = options.Value.ApiKey;
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            logger.LogWarning("OpenRouteService ApiKey is not configured — road distance cannot be determined.");
            return null;
        }

        // ORS expects coordinates as "longitude,latitude" (GeoJSON order)
        var start = $"{fromLng.ToString("F6", CultureInfo.InvariantCulture)},{fromLat.ToString("F6", CultureInfo.InvariantCulture)}";
        var end   = $"{toLng.ToString("F6", CultureInfo.InvariantCulture)},{toLat.ToString("F6", CultureInfo.InvariantCulture)}";

        var url = $"{DirectionsBaseUrl}?api_key={apiKey}&start={start}&end={end}";

        try
        {
            using var response = await httpClient.GetAsync(url, ct);
            if (!response.IsSuccessStatusCode)
            {
                logger.LogWarning("ORS Directions API returned {StatusCode}.", response.StatusCode);
                return null;
            }

            using var stream = await response.Content.ReadAsStreamAsync(ct);
            using var doc = await JsonDocument.ParseAsync(stream, cancellationToken: ct);

            var meters = doc.RootElement
                .GetProperty("features")[0]
                .GetProperty("properties")
                .GetProperty("summary")
                .GetProperty("distance")
                .GetDecimal();

            return Math.Round(meters / 1000m, 2);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error calling ORS Directions API.");
            return null;
        }
    }
}
