# 📦 Detailly – Booking Module (Backend Documentation)

This document describes the current state of the **Booking system backend** in Detailly.

It serves as a reference when implementing the frontend and for future backend expansion.

---

# 🧱 Architecture Overview

Booking logic follows **Clean Architecture**:

- **Domain** → Entities & enums
- **Application**
  - Commands
  - Queries
  - Business rules
  - Abstractions (interfaces)
- **Infrastructure**
  - EF Core
  - Stripe integration
  - Quote service implementation
- **API**
  - Controllers

---

# 🔄 Booking Lifecycle

```
Draft (optional future)
   ↓
PendingPayment (Hold)
   ↓
Confirmed
   ↓
Completed
   ↓
Cancelled / Expired
```

---

# 📌 Core Concepts

## ⏳ Hold-Based Booking

Bookings are created as **PendingPayment** with:

- `ReservationExpiresAtUtc = now + 10 minutes`
- Capacity is reserved atomically
- Payment must be completed before expiry

If expired → availability ignores it.

---

# 📚 Commands

---

## 1️⃣ CreateBookingHoldCommand

**Purpose:**  
Creates a temporary booking reservation.

**Validations:**
- User must be authenticated
- Start time aligned to 30-minute boundary
- Start time must be in the future
- Mobile bookings require address
- Capacity check:
  - Employees
  - Bays (if InShop)

**Uses:**
- `IBookingQuoteService` (centralized duration/price logic)

**Creates:**
- BookingEntity
- BookingItemEntity snapshots (addons)
- BookingVehicleAssignmentEntity

**Status after creation:**
```
PendingPayment
```

---

## 2️⃣ ConfirmBookingAfterPaymentCommand

**Purpose:**
Finalizes booking after successful payment.

**Triggered by:**
- Wallet payment
- Stripe webhook/payment confirmation

**Effects:**
- Status → Confirmed
- Clears ReservationExpiresAtUtc

---

## 3️⃣ CancelBookingCommand

**Allowed from:**
- Draft
- PendingPayment
- Confirmed

**Not allowed from:**
- Completed
- Expired

**Refund Policy:**

| Time Before Start | Refund |
|-------------------|--------|
| ≥ 48h             | 100%   |
| ≥ 24h             | 50%    |
| > 0h              | 25%    |
| ≤ 0h              | 0%     |

Refund automatically triggers:
- `RefundWalletPaymentCommand`
- OR `RefundCardPaymentCommand`

---

## 4️⃣ PayBookingWithWalletCommand

**Flow:**
- Deduct wallet balance
- Create PaymentTransaction (Paid)
- Trigger ConfirmBookingAfterPaymentCommand

---

## 5️⃣ CreateCardPaymentIntentCommand

**Flow:**
- Creates Stripe PaymentIntent
- Creates PaymentTransaction (Pending)
- Returns `ClientSecret`

Stripe confirmation is required to finalize booking.

---

# 🔎 Queries

---

## 1️⃣ GetAvailabilityQuery

**Purpose:**
Returns available time slots for a given date.

**Now Uses:**
- `IBookingQuoteService`
- `LocationOpeningHours`
- Capacity calculation:
  - Employees
  - Bays
  - Blocking bookings

**Opening Hours Logic:**
- Reads from `LocationOpeningHours`
- If `IsClosed` → returns empty list
- Uses `OpenTimeUtc` / `CloseTimeUtc`

Slots generated every 30 minutes.

---

## 2️⃣ GetBookingByIdQuery

Returns:

- Booking details
- Add-ons
- VehicleIds
- PaymentTransactionId
- PaymentStatus

Only accessible by booking owner.

---

# 💰 Payment Transactions

Booking → 1:N PaymentTransactions

Types:
- Payment
- Refund

Provider:
- Wallet
- Stripe

Important:
- Stripe refund uses Stripe **refund ID (re_...)**
- Unique index on `ProviderTransactionId`

---

# 🧮 BookingQuoteService

Located in:
- Interface → `Application/Abstractions/Booking`
- Implementation → `Infrastructure`

Purpose:
Centralize calculation of:

- TotalDurationMinutes
- RequiredEmployees
- RequiredBays
- TotalPrice
- Validated Addons

Used in:
- CreateBookingHoldCommandHandler
- GetAvailabilityQueryHandler

Prevents duplication and drift bugs.

---

# 🕒 Opening Hours

Table:
```
LocationOpeningHours
```

Columns:
- ShopLocationId
- DayOfWeek (0–6)
- OpenTimeUtc
- CloseTimeUtc
- IsClosed

Used in:
- GetAvailabilityQueryHandler

No holidays implemented (yet).

---

# 🧠 Business Rules Summary

- Booking times aligned to 30 min
- Capacity checked atomically inside transaction
- PendingPayment blocks capacity until expiry
- Confirmed blocks capacity permanently
- Completed cannot be cancelled
- Refunds auto-trigger on cancel
- Wallet and Stripe fully supported
- Unique Stripe ProviderTransactionId enforced

---

# 🔐 Security

- Ownership checks on:
  - CancelBooking
  - GetBookingById
- Wallet payments tied to authenticated user
- Stripe payment intent tied to booking + user

---

# 📦 What Frontend Needs

To build Booking UI:

### Step 1 – Availability Screen
Call:
```
GET /Bookings/availability
```

### Step 2 – Create Hold
Call:
```
POST /Bookings
```

Returns BookingId.

### Step 3 – Payment
Either:
```
POST /Payments/bookings/{bookingId}/wallet
```
OR
```
POST /Payments/bookings/{bookingId}/card-intent
```

Then confirm via Stripe.

### Step 4 – View Booking
```
GET /Bookings/{id}
```

---

# 🚀 Future Improvements (Optional)

- Holidays table
- Per-location time zones
- Employee assignment tracking
- Partial refunds by admin
- Rescheduling
- Booking editing
- Background job to auto-expire holds
- Stripe webhook validation hardening

---

# 📌 Current Status

Booking backend is:

✅ Transaction-safe  
✅ Capacity-aware  
✅ Refund-policy-aware  
✅ Opening-hours-aware  
✅ Quote-centralized  
✅ Clean-architecture aligned  

---

End of documentation.