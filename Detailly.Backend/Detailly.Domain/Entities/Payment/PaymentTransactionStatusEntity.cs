using Detailly.Domain.Common;

namespace Detailly.Domain.Entities.Payment
{
    public class PaymentTransactionStatusEntity : BaseEntity
    {
        public string? Name { get; set; } // Pending, Completed, Failed -> prebaciti u enum

        // Foreign keys
        public IReadOnlyCollection<PaymentTransactionEntity> PaymentTransactions { get; private set; } = new List<PaymentTransactionEntity>();
    }
}
