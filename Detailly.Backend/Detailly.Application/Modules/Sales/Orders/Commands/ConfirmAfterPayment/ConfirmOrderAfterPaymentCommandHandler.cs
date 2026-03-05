using Detailly.Domain.Common.Enums;

namespace Detailly.Application.Modules.Sales.Orders.Commands.ConfirmAfterPayment;

public sealed class ConfirmOrderAfterPaymentCommandHandler(IAppDbContext context)
    : IRequestHandler<ConfirmOrderAfterPaymentCommand, Unit>
{
    public async Task<Unit> Handle(ConfirmOrderAfterPaymentCommand request, CancellationToken ct)
    {
        await using var tx = await context.Database.BeginTransactionAsync(ct);

        var payment = await context.PaymentTransactions
            .Include(x => x.Order)
                .ThenInclude(o => o!.OrderItems)
            .FirstOrDefaultAsync(x => x.Id == request.PaymentTransactionId && !x.IsDeleted, ct);

        if (payment is null)
            throw new DetaillyNotFoundException("Payment transaction not found.");

        if (payment.Status != PaymentTransactionStatus.Paid)
            throw new DetaillyBusinessRuleException("PAYMENT_NOT_PAID", "Payment is not marked as paid.");

        if (payment.Order is null)
            return Unit.Value; // not an order payment

        var order = payment.Order;

        // idempotent
        if (order.Status == OrderStatus.Paid)
            return Unit.Value;

        if (order.Status != OrderStatus.PendingPayment)
            throw new DetaillyBusinessRuleException("ORDER_NOT_CONFIRMABLE", "Order is not pending payment.");

        // Load inventories for ordered products
        var productIds = order.OrderItems.Select(i => i.ProductId).Distinct().ToList();

        var products = await context.Products
            .Include(p => p.Inventory)
            .Where(p => productIds.Contains(p.Id))
            .ToListAsync(ct);

        // Validate stock again (race-safe)
        foreach (var item in order.OrderItems)
        {
            var product = products.First(p => p.Id == item.ProductId);

            if (!product.IsEnabled)
                throw new DetaillyBusinessRuleException("PRODUCT_DISABLED", $"Product '{product.Name}' is disabled.");

            if (product.Inventory.QuantityInStock < item.Quantity)
                throw new DetaillyBusinessRuleException("INSUFFICIENT_STOCK", $"Insufficient stock for '{product.Name}'.");
        }

        // Decrement stock
        foreach (var item in order.OrderItems)
        {
            var product = products.First(p => p.Id == item.ProductId);
            product.Inventory.QuantityInStock -= item.Quantity;
        }

        order.Status = OrderStatus.Paid;
        order.ModifiedAtUtc = DateTime.UtcNow;

        await context.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);

        return Unit.Value;
    }
}