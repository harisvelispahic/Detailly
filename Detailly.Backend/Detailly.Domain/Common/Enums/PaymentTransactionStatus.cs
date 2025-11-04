
namespace Detailly.Domain.Common.Enums;
public enum PaymentTransactionStatus
{
    Unpaid = 0,     // Još nije plaćeno
    Pending = 1,    // Plaćanje je u toku (npr. čeka potvrdu)
    Paid = 2,       // Uspješno plaćeno
    Failed = 3,     // Plaćanje nije uspjelo
    Refunded = 4
}


