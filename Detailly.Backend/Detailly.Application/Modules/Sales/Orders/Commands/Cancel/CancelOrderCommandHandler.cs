using Detailly.Application.Modules.Payment.Card.Commands.RefundStripePayment;
using Detailly.Domain.Common.Enums;

namespace Detailly.Application.Modules.Sales.Orders.Commands.Cancel;

public sealed class CancelOrderCommandHandler(IAppDbContext context, IAppAuthorizationService authService, IMediator mediator)
    : IRequestHandler<CancelOrderCommand>
{
    public async Task Handle(CancelOrderCommand request, CancellationToken ct)
    {
        var now = DateTime.UtcNow;

        authService.EnsureAuthenticated();

        var order = await context.Orders
            .FirstOrDefaultAsync(o => o.Id == request.Id && !o.IsDeleted, ct);

        if (order is null)
            throw new DetaillyNotFoundException("Order was not found.");

        authService.EnsureOwnerOrAnyStaff(order.ApplicationUserId, "order");

        // Idempotent
        if (order.Status == OrderStatus.Cancelled)
            return;

        if (order.Status == OrderStatus.Delivered)
            throw new DetaillyBusinessRuleException("order.not_cancellable", "Delivered orders cannot be cancelled.");

        if (order.Status is not (OrderStatus.PendingPayment or OrderStatus.Paid or OrderStatus.Shipped))
            throw new DetaillyBusinessRuleException("order.not_cancellable", "Order cannot be cancelled in its current state.");

        await using var tx = await context.Database.BeginTransactionAsync(ct);

        decimal refundAmount = 0m;
        string? originalState = order.Status.ToString();

        if (order.Status is OrderStatus.Paid or OrderStatus.Shipped)
        {
            var paidPayment = await context.PaymentTransactions
                .AsNoTracking()
                .Where(x =>
                    !x.IsDeleted &&
                    x.OrderId == order.Id &&
                    x.TransactionType == TransactionType.Payment &&
                    x.Status == PaymentTransactionStatus.Paid)
                .OrderByDescending(x => x.TransactionDate)
                .ThenByDescending(x => x.Id)
                .FirstOrDefaultAsync(ct);

            if (paidPayment is null)
                throw new DetaillyBusinessRuleException("order.payment_missing", "Cannot refund: order has no paid payment transaction.");

            if (!string.Equals(paidPayment.Provider, "Stripe", StringComparison.OrdinalIgnoreCase))
                throw new DetaillyBusinessRuleException("order.refund_provider_not_supported", "Only Stripe refunds are supported for orders.");

            var refundPercent = GetRefundPercent(order.Status);
            refundAmount = Math.Round(order.TotalAmount * refundPercent, 2, MidpointRounding.AwayFromZero);

            if (refundAmount > 0)
            {
                await mediator.Send(new RefundStripePaymentCommand(paidPayment.Id, refundAmount), ct);
            }
        }

        order.Status = OrderStatus.Cancelled;
        order.ModifiedAtUtc = now;

        var userReason = request.Reason?.Trim();
        var cancellationMessage = $"[CANCELLED] Order was in state '{originalState}'.";

        if (refundAmount > 0)
            cancellationMessage += $" Refunded {refundAmount:0.00} BAM according to refund policy.";
        else if (order.Status == OrderStatus.PendingPayment)
            cancellationMessage += " No refund issued (payment not completed).";

        if (!string.IsNullOrWhiteSpace(userReason))
            cancellationMessage += $" Reason: {userReason}";

        order.Notes = string.IsNullOrWhiteSpace(order.Notes)
            ? cancellationMessage
            : order.Notes + Environment.NewLine + cancellationMessage;

        await context.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);
    }

    private static decimal GetRefundPercent(OrderStatus status)
    {
        return status switch
        {
            OrderStatus.Paid => 1.00m,
            OrderStatus.Shipped => 0.75m,
            _ => 0.00m
        };
    }
}
