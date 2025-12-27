
namespace Detailly.Domain.Common.Enums;
public enum PaymentTransactionStatus
{
    Unpaid = 0,    // Created internally, no payment started
    Pending = 1,   // External provider processing
    Paid = 2,      // Fully paid (atomic success)
    Failed = 3,    // Payment failed
    Refunded = 4   // Full refund issued
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
//  └─→ Refunded
   
// Failed
//  └─→ (no transitions)
   
// Refunded
//  └─→ (no transitions)
