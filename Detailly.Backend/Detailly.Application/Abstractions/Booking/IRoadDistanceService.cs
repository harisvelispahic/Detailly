namespace Detailly.Application.Abstractions.Booking;

public interface IRoadDistanceService
{
    /// <summary>
    /// Returns the road distance in kilometres between two coordinates.
    /// Returns null when the API call fails or the route cannot be determined
    /// (callers should apply a configured fallback surcharge in that case).
    /// </summary>
    Task<decimal?> GetRoadDistanceKmAsync(
        decimal fromLat, decimal fromLng,
        decimal toLat, decimal toLng,
        CancellationToken ct);
}
