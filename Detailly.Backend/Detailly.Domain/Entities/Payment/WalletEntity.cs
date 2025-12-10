using Detailly.Domain.Common;
using Detailly.Domain.Common.Enums;
using Detailly.Domain.Entities.Booking;
using Detailly.Domain.Entities.Identity;

namespace Detailly.Domain.Entities.Payment;

public class WalletEntity : BaseEntity
{
    public decimal Balance { get; set; } = 0m;
    public CurrencyName Currency { get; set; } = CurrencyName.BAM;
    public decimal TotalDeposited { get; set; } = 0m;
    public int PercentageAdded { get; set; } = 10;
    // sistem kao ACC, korisniku se na uplaceni iznos dodaje x% (npr. 10%) kao bonus

    // Foreign keys
    public int ApplicationUserId { get; set; }   // FK to ApplicationUser
    public ApplicationUserEntity ApplicationUser { get; set; } = null!;

    public IReadOnlyCollection<PaymentTransactionEntity> PaymentTransactions { get; private set; } = new List<PaymentTransactionEntity>();

}
