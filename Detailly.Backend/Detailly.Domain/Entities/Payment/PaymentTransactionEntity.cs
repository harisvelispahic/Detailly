using Detailly.Domain.Common;
using Detailly.Domain.Common.Enums;

namespace Detailly.Domain.Entities.Payment;

public class PaymentTransactionEntity : BaseEntity
{
    public required decimal Amount { get; set; }
    public required TransactionType TransactionType { get; set; } 
    public required DateTime TransactionDate { get; set; }
    public string? Description { get; set; }

    
    // Foreign keys
    public required int WalletId { get; set; }
    public WalletEntity Wallet { get; set; } = null!;
    public PaymentTransactionStatus Status { get; set; } = PaymentTransactionStatus.Unpaid;

}
