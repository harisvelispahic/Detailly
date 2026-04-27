# Booking Module — Backend Reference

Single source of truth for the booking system. Covers every command, every query, the capacity model, the pricing model, and the payment lifecycle. Written to be read by both humans and AI agents implementing adjacent features.

---

## Architecture

Clean Architecture layers:

- **Domain** — `BookingEntity`, enums (`BookingStatus`, `ServiceMode`, `EmployeeWorkMode`)
- **Application** — Commands, Queries, `IBookingQuoteService`, `IRoadDistanceService`
- **Infrastructure** — `BookingQuoteService` (EF + ORS), `RoadDistanceService` (ORS HTTP client)
- **API** — `BookingsController`, `PaymentsController`

CQRS via MediatR. All handlers are `sealed`. No AutoMapper — projections are done inline in each handler.

---

## BookingEntity — Field Reference

```
TotalPrice               decimal   — full price snapshot at hold creation time
MobileSurchargeFee       decimal?  — road-distance surcharge, null for InShop
FleetDiscountPercent     decimal?  — fleet discount percentage applied, null for non-fleet
StartUtc                 DateTime  — service start, aligned to 30-min boundary
EndUtc                   DateTime  — StartUtc + TotalDurationMinutes
RequiredEmployees        int       — number of employee slots reserved
RequiredBays             int       — bay slots (0 for Mobile)
TravelTimeMinutes        int?      — one-way travel shop→address, null for InShop
ReservationExpiresAtUtc  DateTime? — hold expiry (10 min), cleared on Confirmed
Status                   BookingStatus
ServiceMode              ServiceMode (InShop | Mobile)
CustomerId               int
ShopLocationId           int
ServiceAddressId         int?      — required when Mobile
ServicePackageId         int
Notes                    string?
```

Navigation collections: `BookingVehicleAssignments`, `EmployeeAssignments`, `BookingItems`, `PaymentTransactions`.

---

## Booking Lifecycle

```
PendingPayment  →  Confirmed  →  Completed
      ↓               ↓
  Cancelled       Cancelled
      ↓
   Expired  (set externally — hold TTL passed, no payment)
```

State transition rules (enforced per handler):

| From           | To        | Who      | Condition                                    |
| -------------- | --------- | -------- | -------------------------------------------- |
| PendingPayment | Confirmed | System   | Payment recorded as Paid, hold not expired   |
| PendingPayment | Cancelled | Customer | Always (no refund — not paid yet)            |
| Confirmed      | Cancelled | Customer | Refund triggered (see refund policy)         |
| Confirmed      | Completed | Staff    | Employee must be assigned (unless Admin/Mgr) |
| Completed      | —         | —        | Terminal, no transitions                     |
| Expired        | —         | —        | Terminal, no transitions                     |

`CancelBookingCommandHandler` treats `Cancelled`, `Completed`, and `Expired` as idempotent (returns success without error).

---

## Service Modes and Customer Types

Two axes that intersect in every capacity and pricing decision:

**ServiceMode:**

- `InShop` — customer brings vehicle to the shop; uses bays; employees stay on-site
- `Mobile` — team travels to the customer's address; no bays; effective window is widened by travel time

**Customer type:**

- Standard — single vehicle, InShop or Mobile
- Fleet (`IsFleet = true`) — multiple vehicles, **Mobile only** (InShop is forbidden for fleet)

| Mode   | Fleet | Notes                                                               |
| ------ | ----- | ------------------------------------------------------------------- |
| InShop | No    | 1 vehicle, 1 bay, employees stay in shop                            |
| InShop | Yes   | **Forbidden** — rejected at both hold and quote level               |
| Mobile | No    | 1 vehicle, employees travel, effective window = [departure, return] |
| Mobile | Yes   | N vehicles, k-optimization (see below), same widened window         |

---

## BookingQuoteService

**Location:** `Infrastructure/Booking/BookingQuoteService.cs`  
**Interface:** `Application/Abstractions/Booking/IBookingQuoteService.cs`

Single source of truth for price, duration, employee count, bay count, and travel time. Called by `CreateBookingHoldCommandHandler` and `GetAvailabilityQueryHandler`. Never duplicated elsewhere.

### Signature

```csharp
Task<BookingQuoteResult> CalculateAsync(
    int servicePackageId,
    List<int>? addonItemIds,
    ServiceMode serviceMode,
    List<int>? vehicleIds,
    int? customerId,
    bool isFleet,
    CancellationToken ct,
    int? serviceAddressId = null,
    int? shopLocationId = null)
```

`serviceAddressId` and `shopLocationId` are optional — passing both enables the mobile surcharge and travel time calculation. When either is null (e.g., availability check), travel time = 0 and surcharge = 0.

### Calculation steps

**1. Package load** — fetches `ServicePackageEntity`. Throws `DetaillyNotFoundException` if not found.

**2. Fleet/InShop guard** — rejects `isFleet + InShop` immediately.

**3. Add-on validation:**

- Resolves requested addon IDs against `ServicePackageItems` where `IsAddon = true && IsActive = true`
- Rejects any addon already included in the base package's item assignments (`BOOKING_ADDON_DUPLICATE`)
- Rejects missing/inactive addons (`BOOKING_ADDON_INVALID`)
- `addonsDuration` = sum of addon `DurationMinutes`
- `addonsEmployeesMax` = max of addon `RequiredEmployees` (addons can require more staff than the base)
- `addonsPrice` = sum of addon `Price`

**4. Per-vehicle duration:**

```
perVehicleDurationMinutes = package.BaseDurationMinutes + addonsDuration
```

Must be > 0 or throws `BOOKING_DURATION_INVALID`.

**5. Vehicle multiplier (price only):**

- No vehicles → multiplier = 1.0
- Non-fleet with >1 vehicle → `BOOKING_VEHICLE_COUNT_INVALID`
- Validates ownership: all vehicles must belong to `customerId`
- Multiplier = **max** `VehicleCategory.BasePriceMultiplier` across all selected vehicles
- If multiplier ≤ 0, defaults to 1.0

**6. Capacity figures (before k-optimization, done in the hold handler):**

For `InShop + fleet + N > 1 vehicles`:

```
requiredEmployees = max(baseEmployees, addonsEmployeesMax) × N   // parallel teams
requiredBays      = N                                             // each vehicle needs a bay
totalDuration     = perVehicleDurationMinutes                     // all work in parallel
```

For everything else:

```
requiredEmployees = max(baseEmployees, addonsEmployeesMax)
requiredBays      = serviceMode == InShop ? 1 : 0
totalDuration     = perVehicleDurationMinutes
```

Note: For `Mobile + fleet`, these are the _base_ figures. The hold handler overrides `requiredEmployees` and `totalDuration` after running k-optimization.

**7. Fleet discount:**

Only runs when `isFleet = true`. Applied to the service price before the mobile surcharge is added.

```
N = number of vehicles (minimum 1)
discountPercent = min(BaseDiscountPercent + (N - 1) × PerVehicleDiscountPercent, MaxDiscountPercent)
totalPrice *= (1 - discountPercent / 100)
```

Defaults (`appsettings.json["FleetDiscount"]`):

| Setting                  | Default | Effect                                                    |
| ------------------------ | ------- | --------------------------------------------------------- |
| `BaseDiscountPercent`    | 2.0%    | Discount for a 1-vehicle fleet booking                    |
| `PerVehicleDiscountPercent` | 1.0% | Extra discount per additional vehicle beyond the first    |
| `MaxDiscountPercent`     | 8.0%    | Hard cap — reached at 7+ vehicles (2 + 6×1 = 8)          |

Example: 4 vehicles → 2 + 3×1 = 5% discount.

`FleetDiscountPercent` is returned in `BookingQuoteResult` and stored as a snapshot on `BookingEntity` (null for non-fleet).

**8. Mobile surcharge (road distance via ORS):**

Only runs when both `serviceAddressId` and `shopLocationId` are provided:

1. Load `AddressEntity` for the service address
2. Load shop's `Address` coordinates via the `Location` navigation
3. **Cache check:** if `AddressEntity.TravelMetadataLocationId == shopLocationId` and both `DistanceFromShopKm` and `TravelTimeFromShopMinutes` are populated, use cached values (skip ORS call)
4. Otherwise: call `IRoadDistanceService.GetRoadTravelAsync(shopLat, shopLng, addrLat, addrLng)` and persist the result back to the address entity (`DistanceFromShopKm`, `TravelTimeFromShopMinutes`, `TravelMetadataLocationId`) as a best-effort cache write (failures are logged and swallowed)
5. If ORS returns null or coordinates are missing: apply fallback (`OpenRouteServiceOptions.FallbackSurcharge`, `TravelTimeMinutes = 0`)

Surcharge formula:

```
if distanceKm <= FreeRadiusKm → surcharge = 0
else → surcharge = (distanceKm - FreeRadiusKm) × PricePerKm
```

`FreeRadiusKm`, `PricePerKm`, and `FallbackSurcharge` come from `OpenRouteServiceOptions` (bound from `appsettings.json["OpenRouteService"]`).

### BookingQuoteResult fields

```
ServicePackageId           int
TotalDurationMinutes       int    — may be overridden by hold handler's k-optimization
PerVehicleDurationMinutes  int    — always baseDuration + addonsDuration (single vehicle)
RequiredEmployees          int    — may be overridden by hold handler's k-optimization
RequiredBays               int
TotalPrice                 decimal
MobileSurchargeFee         decimal  — 0 for InShop
FleetDiscountPercent       decimal  — 0 for non-fleet; percentage already deducted from TotalPrice
TravelTimeMinutes          int      — 0 for InShop or when address not provided
Addons                     List<AddonSnapshot>
```

---

## Commands

---

### CreateBookingHoldCommand

**File:** `Commands/CreateHold/`  
**Returns:** `int` (new `BookingId`)  
**Auth:** Any authenticated user

**Input:**

```
ServicePackageId  int       required
AddonItemIds      List<int> optional
ServiceMode       enum      required
ShopLocationId    int       required
ServiceAddressId  int?      required when Mobile
StartUtc          DateTime  required
VehicleIds        List<int> optional
Notes             string?   optional
```

**Validation sequence (order matters — fail fast):**

1. User authenticated (`AUTH_REQUIRED`)
2. Fleet + InShop → `FLEET_INSHOP_NOT_ALLOWED`
3. ServiceMode must be InShop or Mobile → `BOOKING_INVALID_MODE`
4. `StartUtc.Second == 0 && StartUtc.Minute % 30 == 0` → `BOOKING_TIME_INVALID`
5. `StartUtc > now` → `BOOKING_TIME_PAST`
6. InShop: `ServiceAddressId` must be null → `BOOKING_ADDRESS_NOT_ALLOWED`
7. Mobile: `ServiceAddressId` must be provided → `BOOKING_ADDRESS_REQUIRED`
8. Mobile: address must belong to `customerId` → `BOOKING_ADDRESS_FORBIDDEN`
9. Vehicle ownership: all `VehicleIds` must belong to `customerId` → `BOOKING_VEHICLE_INVALID`
10. Quote via `IBookingQuoteService.CalculateAsync(...)` — can throw its own rule errors

**k-Optimization (Mobile + fleet + N > 1 vehicles only):**

Runs after the quote to find the minimum number of employees `k` such that the job completes before the latest available shift ends:

```
departureForOptimisation = StartUtc - travelTimeMinutes
maxShiftEnd = MAX(shift.EndUtc) where shift covers departureForOptimisation window

availableWorkMinutes = (maxShiftEnd - StartUtc).TotalMinutes - travelTimeMinutes

for k = 1 to N × baseEmployeesPerVehicle:
    if k < baseEmployeesPerVehicle:
        timePerVehicle = ceil(baseEmployees × perVehicleDuration / k)
        candidateDuration = N × timePerVehicle
    else:
        teams = k / baseEmployeesPerVehicle
        candidateDuration = ceil(N / teams) × perVehicleDuration

    if candidateDuration <= availableWorkMinutes → optimalK = k, break

→ requiredEmployees = optimalK
→ totalDurationMinutes = optimalDuration
```

Throws `BOOKING_NO_CAPACITY` if no mobile shifts exist for that departure window.  
Throws `BOOKING_NO_TIME` if no k fits within available work time.

**Capacity check (inside transaction):**

Effective window for all capacity math:

```
departureUtc = StartUtc - travelTimeMinutes   // equals StartUtc for InShop
returnUtc    = EndUtc + travelTimeMinutes     // equals EndUtc for InShop
```

Available employees: shifts where `StartUtc <= departureUtc && EndUtc >= returnUtc`, matching `ShopLocationId` and `EmployeeWorkMode`.

Blocking bookings: status `Confirmed`, or `PendingPayment` with `ReservationExpiresAtUtc > now`, same `ShopLocationId` and `ServiceMode`. SQL pre-filter: `b.StartUtc < returnUtc && b.EndUtc > departureUtc`. In-memory refinement uses each blocker's own `TravelTimeMinutes`:

```
bDeparture = b.StartUtc - (b.TravelTimeMinutes ?? 0)
bReturn    = b.EndUtc   + (b.TravelTimeMinutes ?? 0)
conflict   = bDeparture < returnUtc && bReturn > departureUtc
```

Errors:

- `BOOKING_NO_CAPACITY` — not enough employees
- `BOOKING_NO_BAYS` — not enough bays (InShop only)
- `DetaillyNotFoundException` — shop location not found

**Entities written (single SaveChanges):**

- `BookingEntity` — status `PendingPayment`, `ReservationExpiresAtUtc = now + 10 min`
- `BookingItemEntity` — one per addon (snapshot of price/duration/employees)
- `BookingVehicleAssignmentEntity` — one per vehicle

EF Core resolves all FK relationships from navigation properties in a single save (no intermediate `SaveChangesAsync` needed).

---

### ConfirmBookingAfterPaymentCommand

**File:** `Commands/ConfirmAfterPayment/`  
**Triggered by:** `PayBookingWithWalletCommandHandler` and `HandleStripeWebhookCommandHandler` (not called directly by the customer)

Looks up the `PaymentTransaction` → loads the associated `Booking` via navigation:

- `PaymentTransactionStatus != Paid` → `PAYMENT_NOT_PAID`
- `Booking.Status == Confirmed` → idempotent return
- `Booking.Status != PendingPayment` → `BOOKING_NOT_CONFIRMABLE`
- `ReservationExpiresAtUtc` null or past → `BOOKING_EXPIRED`

On success: `Status = Confirmed`, `ReservationExpiresAtUtc = null`.

---

### CancelBookingCommand

**File:** `Commands/Cancel/`  
**Auth:** Booking owner only

- Terminal states (`Cancelled`, `Completed`, `Expired`) → idempotent success
- `Confirmed` → triggers refund before setting `Cancelled`
- `PendingPayment` → set `Cancelled` directly (no payment to refund)

**Refund policy (Confirmed only):**

Finds the latest `PaymentTransaction` where `TransactionType = Payment && Status = Paid`. Calculates refund percent from `hoursUntilStart = StartUtc - now`:

| Hours until start | Refund |
| ----------------- | ------ |
| ≥ 48h             | 100%   |
| ≥ 24h             | 50%    |
| > 0h              | 25%    |
| ≤ 0h              | 0%     |

`refundAmount = round(TotalPrice × percent, 2, AwayFromZero)`

If `refundAmount > 0`:

- Wallet payment → `RefundWalletPaymentCommand`
- Stripe payment → `RefundStripePaymentCommand`

No explicit transaction in the cancel handler (refund sub-handlers manage their own transactions).

---

### CompleteBookingCommand

**File:** `Commands/Complete/`  
**Auth:** Employee, Manager, or Admin

- Only `Confirmed` → `Completed` (idempotent if already `Completed`)
- Admin/Manager can complete any confirmed booking
- Employee can only complete if they are in `booking.EmployeeAssignments`; if no assignments exist, throws `BOOKING_NOT_ASSIGNED`

---

### AssignEmployeesToBookingCommand

**File:** `Commands/AssignEmployees/`  
**Auth:** Manager or Admin

**Input:**

```
BookingId    int
EmployeeIds  List<int>
```

Semantics: **replace** — existing assignments are removed and replaced with the new list.

**Validation:**

1. At least one employee must be provided (`ASSIGN_EMPTY`)
2. `employeeIds.Count > booking.RequiredEmployees` → `ASSIGN_TOO_MANY`
3. All employees must exist and be `IsEmployee = true && IsEnabled = true` → `ASSIGN_EMPLOYEE_INVALID`
4. Shift coverage: each employee must have a shift covering `[departureUtc, returnUtc]` matching `ShopLocationId` and `EmployeeWorkMode` → `ASSIGN_NOT_ON_SHIFT`
5. Overlap check: no employee may already be assigned to another `Confirmed/Completed` booking whose effective window overlaps `[departureUtc, returnUtc]`

Overlap check uses the same SQL pre-filter + in-memory refinement pattern as `CreateHold`:

```
SQL pre-filter: a.Booking.StartUtc < returnUtc && a.Booking.EndUtc > departureUtc
In-memory:      bDep = b.StartUtc - (b.TravelTimeMinutes ?? 0)
                bRet = b.EndUtc   + (b.TravelTimeMinutes ?? 0)
                conflict = bDep < returnUtc && bRet > departureUtc
```

→ `ASSIGN_OVERLAP` if any employee conflicts.

Runs inside an explicit transaction.

---

## Queries

---

### GetAvailabilityQuery

**File:** `Queries/GetAvailability/`  
**Auth:** Public (no auth check)

**Input:**

```
DateUtc          DateTime   — only the date part is used
ServicePackageId int
AddonItemIds     List<int>?
ServiceMode      enum
ShopLocationId   int
```

No `VehicleIds` or `ServiceAddressId` — the quote is called with `vehicleIds: null`, `isFleet: false`, `serviceAddressId: null`, `shopLocationId: null`. This means:

- Vehicle multiplier = 1.0 (no vehicle-specific pricing)
- Travel time = 0, mobile surcharge = 0
- The availability result is **approximate for mobile** (it cannot know the caller's travel time without an address)

**Algorithm:**

1. Get `LocationOpeningHours` for the date's `DayOfWeek`. If missing or `IsClosed` → return empty
2. Build day window: `[OpenTimeUtc, CloseTimeUtc]`
3. Fetch `TotalBays` for InShop (if 0 or location missing → return empty)
4. Pre-fetch blocking bookings for the day with status `Confirmed` or active `PendingPayment` — selects `StartUtc`, `EndUtc`, `RequiredEmployees`, `RequiredBays`, `TravelTimeMinutes`
5. Pre-fetch employee shifts where mode and location match and shift overlaps the day window
6. Iterate candidate start times every 30 minutes from `windowStart` to `windowEnd - totalDuration`:
   - Skip past starts (`< now`)
   - `availableEmployees` = distinct employees whose shift covers `[start, end]` exactly
   - `usedEmployees` = sum of `RequiredEmployees` from blocking bookings whose **effective window** `[b.StartUtc - TravelTime, b.EndUtc + TravelTime]` overlaps `[start, end]`
   - If `availableEmployees - usedEmployees < requiredEmployees` → skip
   - InShop: same bay check using `usedBays`
   - Otherwise: emit `{ StartUtc, EndUtc }`

**Returns:** `List<GetAvailabilityQueryDto>` — `{ StartUtc, EndUtc }` per available slot.

The hard capacity gate is in `CreateBookingHoldCommandHandler` (inside a transaction). Availability is a best-effort display.

---

### GetBookingByIdQuery

**File:** `Queries/GetById/`  
**Auth:** Booking owner only

**Returns `GetBookingByIdQueryDto`:**

```
Id                      int
Status                  BookingStatus
ServiceMode             ServiceMode
StartUtc                DateTime
EndUtc                  DateTime
TotalPrice              decimal
MobileSurchargeFee      decimal?   — null for InShop
RequiredEmployees       int
RequiredBays            int
TravelTimeMinutes       int?       — null for InShop
DepartureUtc            DateTime?  — StartUtc - TravelTimeMinutes, null if TravelTimeMinutes = 0
ReturnUtc               DateTime?  — EndUtc + TravelTimeMinutes, null if TravelTimeMinutes = 0
ReservationExpiresAtUtc DateTime?
Notes                   string?
ServicePackageId        int
ServicePackageName      string
PaymentTransactionId    int?       — latest Payment-type transaction
PaymentStatus           PaymentTransactionStatus?
Addons                  List<BookingAddonDto>
VehicleIds              List<int>
```

`DepartureUtc` and `ReturnUtc` are computed in the handler — not stored on the entity. They are non-null only when `TravelTimeMinutes > 0`.

---

### ListMyBookingsQuery

**File:** `Queries/ListMine/`  
**Auth:** Authenticated customer

Paginated, ordered by `StartUtc DESC`. Returns: `{ Id, Status, StartUtc, EndUtc, TotalPrice, ServicePackageName }`.

No status filter — returns all statuses (including Cancelled, Expired). Frontend filters as needed.

---

### ListBookingsForDateQuery (Staff View)

**File:** `Queries/ListForDate/`  
**Auth:** Employee, Manager, or Admin

**Input:**

```
DateUtc                    DateTime
ShopLocationId             int
ServiceMode                enum
IncludePendingPaymentHolds bool  (default false)
Paging                     BasePagedQuery
```

Status filter: always includes `Confirmed` and `Completed`. If `IncludePendingPaymentHolds = true`, also includes `PendingPayment` where `ReservationExpiresAtUtc > now`.

**Returns `ListBookingsForDateQueryDto`:**

```
Id                   int
Status               BookingStatus
StartUtc / EndUtc    DateTime
RequiredEmployees    int
RequiredBays         int
CustomerName         string
ServicePackageName   string
Notes                string?
ReservationExpiresAtUtc  DateTime?
AssignedEmployees    List<{ EmployeeId, FullName }>
```

Ordered by `StartUtc ASC`. Paginated.

---

### ListAssignableEmployeesForBookingQuery

**File:** `Queries/ListAssignableEmployees/`  
**Auth:** Manager or Admin

**Input:** `BookingId int`

Returns employees who:

1. Have a shift with the correct `EmployeeWorkMode` and `ShopLocationId`
2. Shift covers the booking's full effective window `[departureUtc, returnUtc]`
3. Are `IsEmployee = true && IsEnabled = true`
4. Are not already in `booking.EmployeeAssignments`

Each result also includes `HasOverlappingAssignment` — true if the employee is already on another `Confirmed/Completed` booking whose effective window overlaps this booking's `[departureUtc, returnUtc]`. This is informational only; the manager can still assign them (the hard block is in `AssignEmployeesToBookingCommandHandler`).

Overlap detection uses SQL pre-filter + in-memory refinement with each other booking's own `TravelTimeMinutes` (same pattern as `CreateHold`).

Returns `List<{ EmployeeId, FullName, HasOverlappingAssignment }>` ordered by `FullName`.

---

## Payment Lifecycle

Booking payment is handled in `Application/Modules/Payment/`. Two paths converge on `ConfirmBookingAfterPaymentCommand`.

### Wallet payment (synchronous)

```
POST /Payments/bookings/{bookingId}/wallet
→ PayBookingWithWalletCommandHandler
  1. Validates booking is PendingPayment and hold not expired
  2. Rejects if a Pending payment already exists (< 10s old)
  3. Deducts wallet.Balance by booking.TotalPrice
  4. Creates PaymentTransaction (Provider="Wallet", Status=Paid)
  5. SaveChangesAsync
  6. Calls ConfirmBookingAfterPaymentCommand → Status = Confirmed
```

### Stripe payment (asynchronous)

```
POST /Payments/bookings/{bookingId}/card-intent
→ CreateBookingPaymentIntentCommandHandler
  1. Validates PendingPayment and hold not expired
  2. If stale Pending intent (> 10s old) → marks it Failed, allows new intent
  3. Calls IStripeService.CreateBookingPaymentIntentAsync
  4. Creates PaymentTransaction (Provider="Stripe", Status=Pending)
  5. Returns { ClientSecret }

Client confirms payment via Stripe SDK.

Stripe fires webhook →
→ HandleStripeWebhookCommandHandler
  → Marks PaymentTransaction.Status = Paid
  → Calls ConfirmBookingAfterPaymentCommand → Status = Confirmed
```

### PaymentTransaction schema

```
Amount               decimal
TransactionType      Payment | Refund
Status               Pending | Paid | Failed | Unpaid
Provider             "Wallet" | "Stripe"
ProviderTransactionId  string? — Stripe pi_... or re_... (unique index)
WalletId             int?   — set for Wallet payments
BookingId            int?   — set for booking payments
TransactionDate      DateTime
```

---

## Capacity Model — Detailed

### Effective windows

| Mode   | Capacity window                                              |
| ------ | ------------------------------------------------------------ |
| InShop | `[StartUtc, EndUtc]`                                         |
| Mobile | `[StartUtc - TravelTimeMinutes, EndUtc + TravelTimeMinutes]` |

`TravelTimeMinutes` is stored on `BookingEntity` and is always the one-way travel time (shop → address). The effective window is widened by this amount on both sides because employees depart early and return late.

This widened window is used consistently in:

- `CreateBookingHoldCommandHandler` — capacity check
- `AssignEmployeesToBookingCommandHandler` — shift coverage and overlap check
- `ListAssignableEmployeesForBookingQueryHandler` — shift coverage and overlap check
- `GetAvailabilityQueryHandler` — blocker overlap check (but NOT candidate shift coverage, because availability doesn't know the caller's address/travel time)

### Employee shifts

`EmployeeShiftEntity` has `EmployeeWorkMode` (`InShop` | `Mobile`). A shift covers a booking's effective window if:

```
shift.StartUtc <= departureUtc && shift.EndUtc >= returnUtc
```

### Bays

Only relevant for InShop. `Location.TotalBays` is the shop's total bay count. `BookingEntity.RequiredBays` is how many a booking consumes. Mobile bookings always have `RequiredBays = 0`.

### Fleet InShop capacity

Each vehicle is serviced in parallel:

```
requiredEmployees = baseEmployeesPerVehicle × N
requiredBays      = N
totalDuration     = perVehicleDurationMinutes  // not multiplied by N
```

### Fleet Mobile k-optimization

Minimizes the number of employees sent while ensuring all N vehicles can be serviced before the team must depart to return before shift end. Described in detail in the `CreateBookingHoldCommand` section above.

---

## IRoadDistanceService

**Interface:** `Application/Abstractions/Booking/IRoadDistanceService.cs`

```csharp
Task<RoadTravelInfo?> GetRoadTravelAsync(
    decimal fromLat, decimal fromLng,
    decimal toLat, decimal toLng,
    CancellationToken ct);

record RoadTravelInfo(decimal DistanceKm, int TravelTimeMinutes);
```

Implementation calls OpenRouteService (ORS). Returns `null` on failure — callers apply a configured fallback.

Results are cached on `AddressEntity`:

```
DistanceFromShopKm          decimal?
TravelTimeFromShopMinutes   int?
TravelMetadataLocationId    int?   — which shop the cached values are for
```

Cache is per-address-per-shop. If the shop changes, the cache is stale and ORS is called again.

---

## Configuration (`appsettings.json`)

```json
"OpenRouteService": {
  "ApiKey": "",
  "FreeRadiusKm": 10.0,
  "PricePerKm": 0.50,
  "FallbackSurcharge": 5.00
}
```

Bound to `OpenRouteServiceOptions`. Used only by `BookingQuoteService`.

---

## Business Rules Summary

| Rule                           | Detail                                                           |
| ------------------------------ | ---------------------------------------------------------------- |
| Start time alignment           | Must be on 30-minute boundary, seconds = 0                       |
| Start time future              | Must be strictly after `DateTime.UtcNow`                         |
| Fleet + InShop                 | Forbidden — error at both quote and hold level                   |
| Fleet discount                 | 2% base + 1% per vehicle, capped at 8%; applied to service price, not surcharge |
| Mobile requires address        | `ServiceAddressId` required; must be owned by the customer       |
| InShop forbids address         | `ServiceAddressId` must be null                                  |
| Non-fleet vehicle count        | Exactly 1 vehicle if any vehicle is provided                     |
| Hold duration                  | 10 minutes from creation                                         |
| Capacity atomicity             | Employee and bay checks run inside a DB transaction              |
| PendingPayment blocks capacity | Until `ReservationExpiresAtUtc` (active holds count as blockers) |
| Confirmed blocks capacity      | Permanently until the booking ends (or is cancelled)             |
| Refund on cancel               | Only for Confirmed bookings with a Paid transaction              |
| Complete requires assignment   | Employee role requires being in `EmployeeAssignments`            |
| Assign replaces                | Posting new employee list replaces all existing assignments      |

---

## Frontend Integration Points

### Booking flow

```
1. GET  /Bookings/availability?...      → list available { StartUtc, EndUtc } slots
2. POST /Bookings                        → CreateHold → returns BookingId (int)
3a. POST /Payments/bookings/{id}/wallet  → pay with wallet → booking confirmed
3b. POST /Payments/bookings/{id}/card-intent → get Stripe ClientSecret
         → confirm via Stripe.js
         → Stripe fires webhook → booking confirmed asynchronously
4. GET  /Bookings/{id}                  → poll or show booking details
```

### Staff flow

```
GET  /Bookings/for-date?...             → see day's schedule
GET  /Bookings/{id}/assignable-employees → list who can be assigned
POST /Bookings/{id}/assign-employees    → assign staff
POST /Bookings/{id}/complete            → mark completed
```

### Key dates on GetBookingById

For mobile bookings, use `DepartureUtc` and `ReturnUtc` to show when the team leaves and when they return. These are derived from `StartUtc - TravelTimeMinutes` and `EndUtc + TravelTimeMinutes` respectively, and are `null` for InShop bookings.

---

## What Is Intentionally Not Automated

- **Employee scheduling for mobile** — the system reserves `k` employee _slots_ during hold creation, but which specific employees go is a manual manager decision via `AssignEmployeesToBookingCommand`. There is no automatic assignment.
- **Hold expiry** — `ReservationExpiresAtUtc` is set and respected in all queries, but no background job currently soft-deletes or transitions expired holds. Availability and capacity checks filter them out dynamically.
- **Holidays** — opening hours are per day-of-week only. No holiday table exists yet.
- **Per-location timezones** — all times are UTC throughout.
