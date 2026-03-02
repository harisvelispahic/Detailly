
using Detailly.Domain.Common.Enums;
using Detailly.Domain.Entities.Payment;

namespace Detailly.Application.Modules.Payment.Wallet.Commands.RefundWalletPayment;

public class RefundWalletPaymentCommandHandler
    : IRequestHandler<RefundWalletPaymentCommand, Unit>
{
    private readonly IAppDbContext _context;

    public RefundWalletPaymentCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(RefundWalletPaymentCommand request, CancellationToken ct)
    {
        var now = DateTime.UtcNow;

        if (request.Amount <= 0)
            throw new Exception("Refund amount must be greater than zero.");

        var payment = await _context.PaymentTransactions
            .Include(x => x.Wallet)
            .FirstOrDefaultAsync(x => x.Id == request.PaymentTransactionId && !x.IsDeleted, ct)
            ?? throw new Exception("Payment not found.");

        if (payment.Status != PaymentTransactionStatus.Paid)
            throw new Exception("Only paid transactions can be refunded.");

        if (payment.Wallet is null && payment.WalletId is null)
            throw new Exception("This payment is not a wallet payment.");

        if (request.Amount > payment.Amount)
            throw new Exception("Refund amount cannot exceed original payment amount.");

        // Create a separate REFUND transaction (audit-safe)
        var refundTx = new PaymentTransactionEntity
        {
            Amount = request.Amount,
            TransactionType = TransactionType.Refund,
            Status = PaymentTransactionStatus.Refunded,
            TransactionDate = now,

            Provider = "Wallet",
            Description = $"Refund ({request.Amount:0.00}) for payment #{payment.Id}",

            WalletId = payment.WalletId,
            BookingId = payment.BookingId,
            OrderId = payment.OrderId
        };

        _context.PaymentTransactions.Add(refundTx);

        // Credit wallet
        payment.Wallet!.Balance += request.Amount;

        await _context.SaveChangesAsync(ct);

        return Unit.Value;
    }
}