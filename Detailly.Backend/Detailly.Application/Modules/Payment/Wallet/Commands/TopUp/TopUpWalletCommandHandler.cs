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
            .FirstOrDefaultAsync(x => x.ApplicationUserId == request.UserId, ct)
            ?? throw new Exception("Wallet not found.");

        if (request.Amount <= 0)
            throw new Exception("Amount must be greater than zero.");

        // bonus (if enabled)
        var bonus = (wallet.PercentageAdded / 100m) * request.Amount;

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
