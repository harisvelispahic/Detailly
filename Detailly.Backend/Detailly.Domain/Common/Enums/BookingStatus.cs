namespace Detailly.Domain.Common.Enums;

public enum BookingStatus
{
    Draft = 0,          // User is building booking (no payment intent yet)
    PendingPayment = 1, // Payment intent created, waiting for payment
    Confirmed = 2,      // Fully paid & accepted
    Cancelled = 3,      // Cancelled (before or after payment)
    Completed = 4,      // Service finished
    Expired = 5         // Service payment period expired
}

//Draft             ->  Needed for frontend UX(edit freely)
//PendingPayment    ->  Booking exists, payment in progress
//Confirmed         ->  Atomic payment completed
//Cancelled         ->  Terminal, audit-safe
//Completed         ->  Terminal, review enabled


// ==========================================
// ========= ALLOWED TRANSITIONS ============
// ==========================================

// Draft
//  ├─→ PendingPayment
//  └─→ Cancelled

// PendingPayment
//  ├─→ Confirmed
//  ├─→ Expired
//  └─→ Cancelled

// Confirmed
//  ├─→ Completed
//  └─→ Cancelled(business decision)

// Completed
//  └─→ (no transitions)

// Cancelled
//  └─→ (no transitions)
