using Detailly.Domain.Common;
using Detailly.Domain.Common.Enums;
using Detailly.Domain.Entities.Booking;
using Detailly.Domain.Entities.Identity;

namespace Detailly.Domain.Entities.Payment
{
    public class WalletEntity : BaseEntity
    {
        public decimal Balance { get; set; }
        public CurrencyName Currency { get; set; } = CurrencyName.BAM;
        public decimal TotalDeposited { get; set; }
        public int PercentageAdded { get; set; }


        // Foreign keys
        public int ApplicationUserId { get; set; }   // FK to ApplicationUser
        public ApplicationUserEntity ApplicationUser { get; set; } = null!;

        public IReadOnlyCollection<PaymentTransactionEntity> PaymentTransactions { get; private set; } = new List<PaymentTransactionEntity>();

    }
}
