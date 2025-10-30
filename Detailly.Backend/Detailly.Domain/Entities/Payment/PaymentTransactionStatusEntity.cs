
namespace Detailly.Domain.Entities.Payment
{
    public class PaymentTransactionStatusEntity
    {
        public int PaymentTransactionId { get; set; }
        public PaymentTransactionEntity? PaymentTransaction { get; set; }
        public string? StatusCode { get; set; } // Pending, Completed, Failed -> prebaciti u enum
    }
}
