
using Detailly.Application.Abstractions.Payments;
using Detailly.Domain.Common.Enums;
using Detailly.Domain.Entities.Payment;

namespace Detailly.Application.Modules.Payment.Card.Commands.RefundCardPayment;

public sealed class RefundCardPaymentCommandHandler(IAppDbContext context, IStripeService stripe)
    : IRequestHandler<RefundCardPaymentCommand, Unit>
{
    public async Task<Unit> Handle(RefundCardPaymentCommand request, CancellationToken ct)
    {
        var now = DateTime.UtcNow;

        if (request.Amount <= 0)
            throw new Exception("Refund amount must be greater than zero.");

        var payment = await context.PaymentTransactions
            .FirstOrDefaultAsync(x => x.Id == request.PaymentTransactionId && !x.IsDeleted, ct)
            ?? throw new Exception("Payment not found.");

        if (payment.Status != PaymentTransactionStatus.Paid)
            throw new Exception("Only paid transactions can be refunded.");

        if (!string.Equals(payment.Provider, "Stripe", StringComparison.OrdinalIgnoreCase))
            throw new Exception("This payment is not a Stripe payment.");

        if (string.IsNullOrWhiteSpace(payment.ProviderTransactionId))
            throw new Exception("Stripe ProviderTransactionId is missing.");

        if (request.Amount > payment.Amount)
            throw new Exception("Refund amount cannot exceed original payment amount.");

        // Call Stripe refund
        await stripe.RefundPaymentIntentAsync(payment.ProviderTransactionId!, request.Amount, ct);

        // Record refund transaction (audit-safe)
        var refundId = await stripe.RefundPaymentIntentAsync(payment.ProviderTransactionId!, request.Amount, ct);

        var refundTx = new PaymentTransactionEntity
        {
            Amount = request.Amount,
            TransactionType = TransactionType.Refund,
            Status = PaymentTransactionStatus.Refunded,
            TransactionDate = now,

            Provider = "Stripe",
            ProviderTransactionId = refundId, // ✅ re_... not pi_...
            Description = $"Stripe refund ({request.Amount:0.00}) for payment #{payment.Id}",

            BookingId = payment.BookingId,
            OrderId = payment.OrderId
        };

        context.PaymentTransactions.Add(refundTx);

        await context.SaveChangesAsync(ct);

        return Unit.Value;
    }
}