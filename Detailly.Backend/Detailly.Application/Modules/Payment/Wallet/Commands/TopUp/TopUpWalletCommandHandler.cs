using Detailly.Domain.Common.Enums;
using Detailly.Domain.Entities.Payment;

namespace Detailly.Application.Modules.Payment.Wallet.Commands.TopUp;

public class TopUpWalletCommandHandler
    : IRequestHandler<TopUpWalletCommand, Unit>
{
    private readonly IAppDbContext _context;

    public TopUpWalletCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(TopUpWalletCommand request, CancellationToken ct)
    {
        var wallet = await _context.Wallet
            .Include(w => w.ApplicationUser)
            .FirstOrDefaultAsync(x => x.ApplicationUserId == request.UserId, ct)
            ?? throw new DetaillyNotFoundException("Wallet not found.");

        if (request.Amount <= 0)
            throw new DetaillyBusinessRuleException("TOPUP_INVALID_AMOUNT","Amount must be greater than zero.");

        var settings = await _context.SystemSettings.AsNoTracking().FirstOrDefaultAsync(ct);
        var bonusPercent = (wallet.ApplicationUser.IsFleet
            ? settings?.FleetWalletBonusPercent
            : settings?.StandardWalletBonusPercent) ?? 0;

        var bonus = (bonusPercent / 100m) * request.Amount;

        wallet.Balance += request.Amount + bonus;
        wallet.TotalDeposited += request.Amount;

        var transaction = new PaymentTransactionEntity
        {
            Amount = request.Amount,
            TransactionType = TransactionType.Deposit,
            Status = PaymentTransactionStatus.Paid,
            TransactionDate = DateTime.UtcNow,
            Wallet = wallet,
            Provider = "WalletTopUp",
            Description = request.Description
        };

        _context.PaymentTransactions.Add(transaction);

        await _context.SaveChangesAsync(ct);

        return Unit.Value;
    }
}
