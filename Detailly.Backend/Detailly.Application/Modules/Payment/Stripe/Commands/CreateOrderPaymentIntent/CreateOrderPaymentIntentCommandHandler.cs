using Detailly.Application.Abstractions.Payments;
using Detailly.Domain.Common.Enums;
using Detailly.Domain.Entities.Payment;

namespace Detailly.Application.Modules.Payment.Stripe.Commands.CreateOrderPaymentIntent;

public sealed class CreateOrderPaymentIntentCommandHandler(
    IAppDbContext context,
    IStripeService stripe,
    IAppCurrentUser currentUser,
    TimeProvider timeProvider)
    : IRequestHandler<CreateOrderPaymentIntentCommand, CreateOrderPaymentIntentResult>
{
    private static readonly TimeSpan PendingReplaceAfter = TimeSpan.FromMinutes(5);

    public async Task<CreateOrderPaymentIntentResult> Handle(CreateOrderPaymentIntentCommand request, CancellationToken ct)
    {
        if (!currentUser.IsAuthenticated || currentUser.ApplicationUserId is null)
            throw new DetaillyUnauthorizedException("Authentication required.");

        var now = timeProvider.GetUtcNow().UtcDateTime;
        var userId = currentUser.ApplicationUserId.Value;

        var order = await context.Orders
            .FirstOrDefaultAsync(o => o.Id == request.OrderId && !o.IsDeleted, ct)
            ?? throw new DetaillyNotFoundException("Order not found.");

        if (order.ApplicationUserId != userId)
            throw new DetaillyUnauthorizedException("You can only pay your own order.");

        if (order.Status == OrderStatus.Cancelled)
            throw new DetaillyBusinessRuleException("ORDER_CANCELLED", "Order is cancelled.");

        if (order.Status != OrderStatus.PendingPayment)
            throw new DetaillyBusinessRuleException("ORDER_NOT_PAYABLE", "Order is not awaiting payment.");

        // Latest PAYMENT attempt for this order (ignore refunds)
        var existing = await context.PaymentTransactions
            .Where(x =>
                !x.IsDeleted &&
                x.OrderId == order.Id &&
                x.TransactionType == TransactionType.Payment)
            .OrderByDescending(x => x.TransactionDate)
            .ThenByDescending(x => x.Id)
            .FirstOrDefaultAsync(ct);

        if (existing is not null)
        {
            if (existing.Status == PaymentTransactionStatus.Paid)
                throw new DetaillyBusinessRuleException("ORDER_ALREADY_PAID", "Order is already paid.");

            if (existing.Status == PaymentTransactionStatus.Pending)
            {
                var age = now - existing.TransactionDate;

                if (age < PendingReplaceAfter)
                    throw new DetaillyBusinessRuleException("PAYMENT_IN_PROGRESS", "Payment is already in progress.");

                // stale pending -> cancel on Stripe so it can never be completed, then mark failed locally
                await stripe.CancelPaymentIntentAsync(existing.ProviderTransactionId, ct);
                existing.Status = PaymentTransactionStatus.Failed;
                existing.Description = (existing.Description ?? "Order card payment")
                                       + " (auto-failed due to stale pending intent)";
            }

            if (existing.Status is not (PaymentTransactionStatus.Failed or PaymentTransactionStatus.Unpaid or PaymentTransactionStatus.Pending))
                throw new DetaillyBusinessRuleException("PAYMENT_NOT_ALLOWED", "Order cannot start a new payment at this time.");
        }

        var (providerTransactionId, clientSecret) =
            await stripe.CreateOrderPaymentIntentAsync(order.TotalAmount, order.Id, ct);

        var transaction = new PaymentTransactionEntity
        {
            Amount = order.TotalAmount,
            TransactionType = TransactionType.Payment,
            Status = PaymentTransactionStatus.Pending,
            TransactionDate = now,

            Provider = "Stripe",
            ProviderTransactionId = providerTransactionId,
            Description = "Order card payment intent created",

            OrderId = order.Id
        };

        context.PaymentTransactions.Add(transaction);
        await context.SaveChangesAsync(ct);

        return new CreateOrderPaymentIntentResult { ClientSecret = clientSecret };
    }
}