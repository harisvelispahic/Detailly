using Detailly.Domain.Common;

namespace Detailly.Domain.Entities.Payment
{
    public class PaymentTransactionEntity : BaseEntity
    {
        public decimal Amount { get; set; }
        public string? TransactionType { get; set; } // Deposit, Withdrawal, Payment -> prebaciti u enum
        public DateTime TransactionDate { get; set; }
        public string? Description { get; set; }

        
        // Foreign keys
        public int WalletId { get; set; }
        public WalletEntity Wallet { get; set; } = null!;
        public int PaymentTransactionStatusId { get; set; }
        public PaymentTransactionStatusEntity PaymentTransactionStatus { get; set; } = null!;

    }
}
