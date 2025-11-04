using Detailly.Domain.Common;
using Detailly.Domain.Common.Enums;

namespace Detailly.Domain.Entities.Payment
{
    public class PaymentTransactionEntity : BaseEntity
    {
        public decimal Amount { get; set; }
        public TransactionType TransactionType { get; set; } 
        public DateTime TransactionDate { get; set; }
        public string? Description { get; set; }

        
        // Foreign keys
        public int WalletId { get; set; }
        public WalletEntity Wallet { get; set; } = null!;
        public int PaymentTransactionStatusId { get; set; }
        public PaymentTransactionStatus Status { get; set; } = PaymentTransactionStatus.Unpaid;

    }
}
