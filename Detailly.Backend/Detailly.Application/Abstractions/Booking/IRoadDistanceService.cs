namespace Detailly.Application.Abstractions.Booking;

public record RoadTravelInfo(decimal DistanceKm, int TravelTimeMinutes);

public interface IRoadDistanceService
{
    /// <summary>
    /// Returns the road distance in kilometres between two coordinates.
    /// Returns null when the API call fails or the route cannot be determined.
    /// </summary>
    Task<decimal?> GetRoadDistanceKmAsync(
        decimal fromLat, decimal fromLng,
        decimal toLat, decimal toLng,
        CancellationToken ct);

    /// <summary>
    /// Returns both road distance (km) and one-way travel time (minutes) in a single ORS call.
    /// Returns null when the API call fails or the route cannot be determined.
    /// </summary>
    Task<RoadTravelInfo?> GetRoadTravelAsync(
        decimal fromLat, decimal fromLng,
        decimal toLat, decimal toLng,
        CancellationToken ct);
}
