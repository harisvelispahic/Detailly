using Detailly.Domain.Common;
using Detailly.Domain.Common.Enums;
using Detailly.Domain.Entities.Sales;

namespace Detailly.Domain.Entities.Payment;

public class PaymentTransactionEntity : BaseEntity
{
    public required decimal Amount { get; set; }
    public required TransactionType TransactionType { get; set; } 
    public PaymentTransactionStatus Status { get; set; } = PaymentTransactionStatus.Unpaid;
    public required DateTime TransactionDate { get; set; }
    public string? Description { get; set; }

    
    // Foreign keys
    public required int WalletId { get; set; }
    public WalletEntity Wallet { get; set; } = null!;
    public int? OrderId { get; set; }
    public OrderEntity? Order { get; set; }
    public string? Provider { get; set; }               // e.g. "Wallet", "Stripe"
    public string? ProviderTransactionId { get; set; }  // unique reference ID from the external provider, Wallet : null

}
