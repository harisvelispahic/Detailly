using Detailly.Domain.Common;
using Detailly.Domain.Common.Enums;
using Detailly.Domain.Entities.Booking;
using Detailly.Domain.Entities.Sales;

namespace Detailly.Domain.Entities.Payment;

public class PaymentTransactionEntity : BaseEntity
{
    // Core payment data
    public required decimal Amount { get; set; }
    public required TransactionType TransactionType { get; set; } // Credit / Debit
    public PaymentTransactionStatus Status { get; set; } = PaymentTransactionStatus.Unpaid;
    public required DateTime TransactionDate { get; set; }

    // Provider info
    public string? Provider { get; set; }               // Wallet, Stripe, PayPal
    public string? ProviderTransactionId { get; set; }  // External reference
    public string? Description { get; set; }

    // OPTIONAL foreign keys (by design)
    public int? WalletId { get; set; }
    public WalletEntity? Wallet { get; set; }

    public int? BookingId { get; set; }
    public BookingEntity? Booking { get; set; }

    public int? OrderId { get; set; }
    public OrderEntity? Order { get; set; }
}
