namespace Detailly.Domain.Common.Enums;
public enum PaymentTransactionStatus
{
    Unpaid = 0,    // Created internally, no payment started
    Pending = 1,   // External provider processing
    Paid = 2,      // Fully paid (atomic success)
    Failed = 3,    // Payment failed
    Refunded = 4,  // Full refund issued
    PartiallyRefunded = 5 // Refund issued for less than the full amount
}


// Unpaid        ->  useful for wallet/internal intents
// Pending       ->  required for async providers
// Paid          ->  the only success state
// Failed        ->  allows retries
// Refunded      ->  preserves audit history


// ==========================================
// ========= ALLOWED TRANSITIONS ============
// ==========================================

// Unpaid
//  └─→ Pending
   
// Pending
//  ├─→ Paid
//  └─→ Failed
   
// Paid
//  ├─→ Refunded          (full-amount refund)
//  └─→ PartiallyRefunded (refund for less than the full amount)

// Failed
//  └─→ (no transitions)

// Refunded
//  └─→ (no transitions)

// PartiallyRefunded
//  └─→ (no transitions — system allows one refund per payment)
