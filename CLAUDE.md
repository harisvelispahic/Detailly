# Detailly — Project Notes for Claude

## Dates & Timezones

All `DateTime` fields in the domain and DTOs are UTC (field names have the `Utc` suffix, e.g. `StartUtc`, `ReservationExpiresAtUtc`).

The backend serializes with `DateTimeZoneHandling.Utc` (configured in `Detailly.Api/DependencyInjection.cs`), so Newtonsoft.Json always emits the `Z` suffix. EF Core returns `DateTime` with `DateTimeKind.Unspecified`; the serializer setting ensures the wire format is unambiguous UTC.

On the frontend, `new Date(dateStr)` is safe to use because the `Z` suffix is always present. Never append `Z` manually or adjust for local offset — the serializer handles it.
