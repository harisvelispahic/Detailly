using Detailly.Domain.Common.Enums;

namespace Detailly.Application.Modules.Sales.Orders.Commands.Submit;

public sealed class SubmitOrderCommandHandler(IAppDbContext context, IAppAuthorizationService authService)
    : IRequestHandler<SubmitOrderCommand>
{
    public async Task Handle(SubmitOrderCommand request, CancellationToken ct)
    {
        authService.EnsureAuthenticated();

        var order = await context.Orders
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.Id == request.Id, ct);

        if (order is null)
            throw new DetaillyNotFoundException("Order was not found.");

        authService.EnsureOwnerOrStaff(order.ApplicationUserId, "order");

        //if (order.Status != OrderStatus.Draft)
        //    throw new DetaillyBusinessRuleException("order.not_draft", "Only Draft orders can be submitted.");

        if (order.OrderItems.Count == 0)
            throw new DetaillyBusinessRuleException("order.empty", "Cannot submit an empty order.");

        // Optional: re-check inventory right before payment starts (still no decrement)
        var productIds = order.OrderItems.Select(x => x.ProductId).Distinct().ToList();

        var products = await context.Products
            .Include(p => p.Inventory)
            .Where(p => productIds.Contains(p.Id))
            .ToListAsync(ct);

        foreach (var item in order.OrderItems)
        {
            var product = products.First(p => p.Id == item.ProductId);

            if (!product.IsEnabled)
                throw new DetaillyBusinessRuleException("product.disabled", $"Product '{product.Name}' is disabled.");

            if (product.Inventory.QuantityInStock < item.Quantity)
                throw new DetaillyBusinessRuleException("inventory.insufficient", $"Insufficient stock for product '{product.Name}'.");
        }

        order.Status = OrderStatus.PendingPayment;
        await context.SaveChangesAsync(ct);
    }
}