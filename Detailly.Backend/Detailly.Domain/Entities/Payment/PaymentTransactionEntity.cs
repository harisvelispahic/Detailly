using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Detailly.Domain.Entities.Payment
{
    public class PaymentTransactionEntity
    {
        public int WalletId { get; set; }
        public WalletEntity? Wallet { get; set; }
        public decimal Amount { get; set; }
        public string? TransactionType { get; set; } // Deposit, Withdrawal, Payment -> prebaciti u enum
        public DateTime TransactionDate { get; set; }
        public string? Description { get; set; }


        public PaymentTransactionStatusEntity? PaymentTransactionStatus { get; set; }

    }
}
