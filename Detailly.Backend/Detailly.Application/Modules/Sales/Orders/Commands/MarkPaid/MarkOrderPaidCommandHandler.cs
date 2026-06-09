using Detailly.Domain.Common.Enums;
using Detailly.Domain.Entities.Payment;

namespace Detailly.Application.Modules.Sales.Orders.Commands.MarkPaid;

public sealed class MarkOrderPaidCommandHandler(IAppDbContext context, IAppAuthorizationService authService)
    : IRequestHandler<MarkOrderPaidCommand>
{
    public async Task Handle(MarkOrderPaidCommand request, CancellationToken ct)
    {
        authService.EnsureAuthenticated();

        await using var tx = await context.Database.BeginTransactionAsync(ct);

        var order = await context.Orders
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.Id == request.Id, ct);

        if (order is null)
            throw new DetaillyNotFoundException("Order was not found.");

        authService.EnsureOwnerOrStaff(order.ApplicationUserId, "order");

        if (order.Status == OrderStatus.Paid)
            return; // idempotent success

        if (order.Status != OrderStatus.PendingPayment)
            throw new DetaillyBusinessRuleException("order.not_payable", "Only Pending orders can be marked as paid.");

        // Load products + inventories used by the order
        var productIds = order.OrderItems.Select(x => x.ProductId).Distinct().ToList();

        var products = await context.Products
            .Include(p => p.Inventory)
            .Where(p => productIds.Contains(p.Id))
            .ToListAsync(ct);

        // Validate stock first
        foreach (var item in order.OrderItems)
        {
            var product = products.First(p => p.Id == item.ProductId);

            if (!product.IsEnabled)
                throw new DetaillyBusinessRuleException("product.disabled", $"Product '{product.Name}' is disabled.");

            if (product.Inventory.QuantityInStock < item.Quantity)
                throw new DetaillyBusinessRuleException("inventory.insufficient", $"Insufficient stock for product '{product.Name}'.");
        }

        // Decrement stock (simple strategy)
        foreach (var item in order.OrderItems)
        {
            var product = products.First(p => p.Id == item.ProductId);
            product.Inventory.QuantityInStock -= item.Quantity;
        }

        // Create a PaymentTransaction record linked to this order
        var payment = new PaymentTransactionEntity
        {
            Amount = order.TotalAmount,
            TransactionType = TransactionType.Payment,
            Status = PaymentTransactionStatus.Paid,
            TransactionDate = DateTime.UtcNow,
            Provider = request.Provider,
            ProviderTransactionId = request.ProviderTransactionId,
            Description = request.Description,
            OrderId = order.Id
        };

        context.PaymentTransactions.Add(payment);

        // Update order status
        order.Status = OrderStatus.Paid;

        await context.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);
    }
}