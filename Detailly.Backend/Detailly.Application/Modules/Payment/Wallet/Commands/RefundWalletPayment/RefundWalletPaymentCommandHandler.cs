using Detailly.Domain.Common.Enums;
using Detailly.Domain.Entities.Payment;

namespace Detailly.Application.Modules.Payment.Wallet.Commands.RefundWalletPayment;

public class RefundWalletPaymentCommandHandler
    : IRequestHandler<RefundWalletPaymentCommand, Unit>
{
    private readonly IAppDbContext _context;
    private readonly TimeProvider _timeProvider;

    public RefundWalletPaymentCommandHandler(IAppDbContext context, TimeProvider timeProvider)
    {
        _context = context;
        _timeProvider = timeProvider;
    }

    public async Task<Unit> Handle(RefundWalletPaymentCommand request, CancellationToken ct)
    {
        var now = _timeProvider.GetUtcNow().UtcDateTime;
        var idempotencyKey = $"refund:{request.PaymentTransactionId}";

        if (request.Amount <= 0)
            throw new DetaillyBusinessRuleException("INVALID_REFUND_AMOUNT", "Refund amount must be greater than zero.");

        var existingRefund = await _context.PaymentTransactions
            .FirstOrDefaultAsync(x => x.IdempotencyKey == idempotencyKey, ct);
        if (existingRefund is not null)
            return Unit.Value;

        var payment = await _context.PaymentTransactions
            .Include(x => x.Wallet)
            .FirstOrDefaultAsync(x => x.Id == request.PaymentTransactionId && !x.IsDeleted, ct)
            ?? throw new DetaillyBusinessRuleException("PAYMENT_NOT_FOUND", "Payment not found.");

        if (payment.Status != PaymentTransactionStatus.Paid)
            throw new DetaillyBusinessRuleException("ONLY_PAID_REFUNDABLE", "Only paid transactions can be refunded.");

        if (payment.Wallet is null && payment.WalletId is null)
            throw new DetaillyBusinessRuleException("PAYMENT_NOT_WALLET", "This payment is not a wallet payment.");

        if (request.Amount > payment.Amount)
            throw new DetaillyBusinessRuleException("REFUND_AMOUNT_INVALID", "Refund amount cannot exceed original payment amount.");

        var refundTx = new PaymentTransactionEntity
        {
            Amount = request.Amount,
            TransactionType = TransactionType.Refund,
            Status = PaymentTransactionStatus.Refunded,
            TransactionDate = now,
            IdempotencyKey = idempotencyKey,
            Provider = "Wallet",
            Description = $"Refund ({request.Amount:0.00}) for payment #{payment.Id}",
            WalletId = payment.WalletId,
            BookingId = payment.BookingId,
            OrderId = payment.OrderId
        };

        _context.PaymentTransactions.Add(refundTx);

        payment.Wallet!.Balance += request.Amount;
        payment.Status = PaymentTransactionStatus.Refunded;

        await _context.SaveChangesAsync(ct);

        return Unit.Value;
    }
}
