using System.Globalization;
using System.Text.Json;
using Detailly.Application.Abstractions.Booking;
using Detailly.Shared.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Detailly.Infrastructure.Booking;

/// <summary>
/// Calculates road distance and travel time using the OpenRouteService Directions API.
/// ORS uses GeoJSON coordinate order: longitude first, latitude second.
/// Parameters arrive as (lat, lng) but are passed to ORS as "lng,lat".
/// </summary>
public sealed class OrsRoadDistanceService(
    HttpClient httpClient,
    IOptions<OpenRouteServiceOptions> options,
    ILogger<OrsRoadDistanceService> logger) : IRoadDistanceService
{
    private const string DirectionsBaseUrl = "https://api.openrouteservice.org/v2/directions/driving-car";
    private const string GeocodeBaseUrl = "https://api.openrouteservice.org/geocode/search";

    public async Task<decimal?> GetRoadDistanceKmAsync(
        decimal fromLat, decimal fromLng,
        decimal toLat, decimal toLng,
        CancellationToken ct)
    {
        var result = await GetRoadTravelAsync(fromLat, fromLng, toLat, toLng, ct);
        return result?.DistanceKm;
    }

    public async Task<RoadTravelInfo?> GetRoadTravelAsync(
        decimal fromLat, decimal fromLng,
        decimal toLat, decimal toLng,
        CancellationToken ct)
    {
        var apiKey = options.Value.ApiKey;
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            logger.LogWarning("OpenRouteService ApiKey is not configured — road travel info cannot be determined.");
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

            var summary = doc.RootElement
                .GetProperty("features")[0]
                .GetProperty("properties")
                .GetProperty("summary");

            var distanceKm = Math.Round(summary.GetProperty("distance").GetDecimal() / 1000m, 2);
            var durationSeconds = summary.GetProperty("duration").GetDecimal();
            var travelTimeMinutes = (int)Math.Ceiling(durationSeconds / 60m);

            return new RoadTravelInfo(distanceKm, travelTimeMinutes);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error calling ORS Directions API.");
            return null;
        }
    }

    public async Task<GeoCoordinates?> GetCoordinatesAsync(
        string street, string city, string? postalCode, string country,
        CancellationToken ct)
    {
        var apiKey = options.Value.ApiKey;
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            logger.LogWarning("OpenRouteService ApiKey is not configured — geocoding cannot be performed.");
            return null;
        }

        // Build a free-text query: "Street, PostalCode City, Country"
        var parts = new List<string>();
        if (!string.IsNullOrWhiteSpace(street))   parts.Add(street.Trim());
        if (!string.IsNullOrWhiteSpace(postalCode)) parts.Add(postalCode.Trim());
        if (!string.IsNullOrWhiteSpace(city))     parts.Add(city.Trim());
        parts.Add(country.Trim());

        var text = Uri.EscapeDataString(string.Join(", ", parts));
        var url = $"{GeocodeBaseUrl}?api_key={apiKey}&text={text}&size=1";

        try
        {
            using var response = await httpClient.GetAsync(url, ct);
            if (!response.IsSuccessStatusCode)
            {
                logger.LogWarning("ORS Geocode API returned {StatusCode}.", response.StatusCode);
                return null;
            }

            using var stream = await response.Content.ReadAsStreamAsync(ct);
            using var doc = await JsonDocument.ParseAsync(stream, cancellationToken: ct);

            var features = doc.RootElement.GetProperty("features");
            if (features.GetArrayLength() == 0)
            {
                logger.LogWarning("ORS Geocode API returned no results for the given address.");
                return null;
            }

            // GeoJSON: coordinates are [longitude, latitude]
            var coords = features[0]
                .GetProperty("geometry")
                .GetProperty("coordinates");

            var lng = coords[0].GetDecimal();
            var lat = coords[1].GetDecimal();

            return new GeoCoordinates(lat, lng);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error calling ORS Geocode API.");
            return null;
        }
    }
}
