using Detailly.Application.Abstractions.Payments;
using Detailly.Domain.Common.Enums;
using Detailly.Domain.Entities.Payment;
using Microsoft.EntityFrameworkCore.Storage;

namespace Detailly.Application.Modules.Payment.Card.Commands.RefundStripePayment;

public sealed class RefundStripePaymentCommandHandler(
    IAppDbContext context,
    IStripeService stripe,
    TimeProvider timeProvider)
    : IRequestHandler<RefundStripePaymentCommand, Unit>
{
    public async Task<Unit> Handle(RefundStripePaymentCommand request, CancellationToken ct)
    {
        var now = timeProvider.GetUtcNow().UtcDateTime;
        var idempotencyKey = $"refund:{request.PaymentTransactionId}";

        if (request.Amount <= 0)
            throw new DetaillyBusinessRuleException("refund.invalid_amount", "Refund amount must be greater than zero.");

        var existingRefund = await context.PaymentTransactions
            .FirstOrDefaultAsync(x => x.IdempotencyKey == idempotencyKey, ct);
        if (existingRefund is not null)
            return Unit.Value;

        var payment = await context.PaymentTransactions
            .FirstOrDefaultAsync(x => x.Id == request.PaymentTransactionId && !x.IsDeleted, ct);

        if (payment is null)
            throw new DetaillyNotFoundException("Payment not found.");

        if (payment.Status != PaymentTransactionStatus.Paid)
            throw new DetaillyBusinessRuleException("refund.not_paid", "Only paid transactions can be refunded.");

        if (!string.Equals(payment.Provider, "Stripe", StringComparison.OrdinalIgnoreCase))
            throw new DetaillyBusinessRuleException("refund.not_stripe", "This payment is not a Stripe payment.");

        if (string.IsNullOrWhiteSpace(payment.ProviderTransactionId))
            throw new DetaillyBusinessRuleException("refund.missing_provider_id", "Stripe ProviderTransactionId is missing.");

        if (request.Amount > payment.Amount)
            throw new DetaillyBusinessRuleException("refund.exceeds_amount", "Refund amount cannot exceed original payment amount.");

        var ownTx = context.Database.CurrentTransaction is null;
        IDbContextTransaction? tx = ownTx ? await context.Database.BeginTransactionAsync(ct) : null;
        await using (tx)
        {
            var refundId = await stripe.RefundPaymentIntentAsync(payment.ProviderTransactionId!, request.Amount, ct);

            var refundTx = new PaymentTransactionEntity
            {
                Amount = request.Amount,
                TransactionType = TransactionType.Refund,
                Status = PaymentTransactionStatus.Refunded,
                TransactionDate = now,
                IdempotencyKey = idempotencyKey,
                Provider = "Stripe",
                ProviderTransactionId = refundId,
                Description = $"Stripe refund ({request.Amount:0.00}) for payment #{payment.Id}",
                BookingId = payment.BookingId,
                OrderId = payment.OrderId
            };

            context.PaymentTransactions.Add(refundTx);
            payment.Status = PaymentTransactionStatus.Refunded;

            await context.SaveChangesAsync(ct);
            if (ownTx) await tx!.CommitAsync(ct);

            return Unit.Value;
        }
    }
}
