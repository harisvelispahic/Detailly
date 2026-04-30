# Detailly — Project Notes for Claude

## Dates & Timezones

All `DateTime` fields in the domain and DTOs are UTC (field names have the `Utc` suffix, e.g. `StartUtc`, `ReservationExpiresAtUtc`).

The backend serializes with `DateTimeZoneHandling.Utc` (configured in `Detailly.Api/DependencyInjection.cs`), so Newtonsoft.Json always emits the `Z` suffix. EF Core returns `DateTime` with `DateTimeKind.Unspecified`; the serializer setting ensures the wire format is unambiguous UTC.

On the frontend, `new Date(dateStr)` is safe to use because the `Z` suffix is always present. Never append `Z` manually or adjust for local offset — the serializer handles it.

## Date/Time Display Standard

DateTime values are **stored in UTC, but entered and displayed in the user's local time**. This means:

- When displaying a UTC datetime (e.g. `startUtc`), use Angular's `date` pipe **without** a timezone argument so it renders in local time: `{{ s.startUtc | date: 'HH:mm' }}`, not `{{ s.startUtc | date: 'HH:mm' : 'UTC' }}`.
- Column headers and labels must never say "(UTC)".
- When writing datetime values back to the API, construct them from local date/time inputs and let `Date.toISOString()` convert to UTC — do not manually apply offsets.
- **Opening hours** (hour/minute pairs on locations) are **not datetimes**. They are recurring local time-of-day patterns (e.g. "opens at 08:00 every Monday") and are stored as plain integers. They are correct as-is and must not be UTC-converted.

## Custom Date Pipe

All date displays must use the `detailyDate` pipe defined in `Detailly.Frontend/src/app/modules/shared/pipes/detaily-date.pipe.ts` and exported from `SharedModule`.

- **Short date** (`| detailyDate: 'short'` or just `| detailyDate`): `DD/MM/YYYY` — e.g. `07/05/2026`
- **Long date** (`| detailyDate: 'long'`): `DD MonthName YYYY` — e.g. `07 May 2026`

Never use Angular's built-in `date` pipe for displaying calendar dates. Use `detailyDate` instead.

## Angular Material Date Picker Format

`MatNativeDateModule` (the native date adapter) does **not** understand Moment.js-style format strings like `'dd/MM/yyyy'`. It passes the `MAT_DATE_FORMATS` display value directly to `Intl.DateTimeFormat`, so the format must be an `Intl.DateTimeFormatOptions` object, not a string.

The correct setup is already in `SharedModule` (`shared-module.ts`):

- `MAT_DATE_FORMATS.display.dateInput` is `{ year: 'numeric', month: '2-digit', day: '2-digit' }` (not a string).
- `MAT_DATE_LOCALE` is `'en-GB'` so `Intl.DateTimeFormat` orders fields as `DD/MM/YYYY`.

If date pickers start showing `M/D/YYYY` (en-US order), the likely cause is one of these providers being missing or overridden. Do not revert them to format strings.

## Time Display Standard

All time values must be displayed in **24-hour HH:mm format** using Angular's built-in `date` pipe:

```
{{ value | date: 'HH:mm' }}
```

Never use `hour12: true`, AM/PM, or 12-hour formats. No TypeScript `toLocaleString`/`toLocaleDateString` calls for display — use pipes in templates.
