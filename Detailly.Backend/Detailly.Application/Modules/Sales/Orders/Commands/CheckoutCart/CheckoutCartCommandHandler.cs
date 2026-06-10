using Detailly.Domain.Common.Enums;
using Detailly.Domain.Entities.Sales;

namespace Detailly.Application.Modules.Sales.Orders.Commands.CheckoutCart;

public sealed class CheckoutCartCommandHandler(
    IAppDbContext context,
    IAppCurrentUser appCurrentUser,
    TimeProvider timeProvider)
    : IRequestHandler<CheckoutCartCommand, int>
{
    public async Task<int> Handle(CheckoutCartCommand request, CancellationToken ct)
    {
        if (!appCurrentUser.IsAuthenticated || appCurrentUser.ApplicationUserId is null)
            throw new DetaillyUnauthorizedException("User is not authenticated.");

        var userId = appCurrentUser.ApplicationUserId.Value;

        // Validate address exists
        var addressExists = await context.Addresses
            .AnyAsync(a => a.Id == request.ShipToAddressId && a.ApplicationUserId == userId, ct);

        if (!addressExists)
            throw new DetaillyBusinessRuleException("address.not_found", "Shipping address does not exist.");


        await using var tx = await context.Database.BeginTransactionAsync(ct);

        // Load cart + items for this user
        var cart = await context.Carts
            .Include(c => c.CartItems)
            .FirstOrDefaultAsync(c => c.ApplicationUserId == userId, ct);

        if (cart is null)
            throw new DetaillyBusinessRuleException("cart.missing", "Cart does not exist.");

        if (cart.Status != CartStatus.Active)
            throw new DetaillyBusinessRuleException("cart.not_active", "Cart is not active.");

        if (cart.CartItems.Count == 0)
            throw new DetaillyBusinessRuleException("cart.empty", "Cart is empty.");

        // Group by product just in case (defensive)
        var requested = cart.CartItems
            .GroupBy(ci => ci.ProductId)
            .Select(g => new { ProductId = g.Key, Quantity = g.Sum(x => x.Quantity) })
            .ToList();

        if (requested.Any(x => x.Quantity <= 0))
            throw new DetaillyBusinessRuleException("cart.invalid_quantity", "Cart contains an invalid quantity.");

        // Load products + inventory
        var productIds = requested.Select(x => x.ProductId).ToList();

        var products = await context.Products
            .Include(p => p.Inventory)
            .Where(p => productIds.Contains(p.Id))
            .ToListAsync(ct);

        if (products.Count != productIds.Count)
            throw new DetaillyBusinessRuleException("cart.product_missing", "Cart contains a product that no longer exists.");

        if (products.Any(p => !p.IsEnabled))
            throw new DetaillyBusinessRuleException("cart.product_disabled", "Cart contains a disabled product.");

        // Validate stock
        foreach (var req in requested)
        {
            var product = products.First(p => p.Id == req.ProductId);

            if (product.Inventory.QuantityInStock < req.Quantity)
                throw new DetaillyBusinessRuleException(
                    "inventory.insufficient",
                    $"Insufficient stock for product '{product.Name}'.");
        }

        // Create order (Pending) - no Draft in normal flow
        var order = new OrderEntity
        {
            OrderNumber = Guid.NewGuid().ToString("N"),
            OrderDate = timeProvider.GetUtcNow().UtcDateTime,
            Notes = request.Notes?.Trim(),
            ShipToAddressId = request.ShipToAddressId,
            ApplicationUserId = userId,
            Status = OrderStatus.PendingPayment,
            TotalAmount = 0m
        };

        context.Orders.Add(order);
        await context.SaveChangesAsync(ct);

        decimal totalAmount = 0m;

        foreach (var req in requested)
        {
            var product = products.First(p => p.Id == req.ProductId);

            // Server-owned pricing
            var unitPrice = product.Price;
            var currency = product.Currency;

            var lineSubtotal = unitPrice * req.Quantity;

            // Keep your current placeholder discount model.
            // Later: Promotion engine; for now it's deterministic server-side.
            //var discount = 0.05m;
            var discount = 0m;

            var lineTotal = (1 - discount) * lineSubtotal;

            var orderItem = new OrderItemEntity
            {
                OrderId = order.Id,
                ProductId = product.Id,

                UnitPrice = unitPrice,
                Currency = currency,
                Quantity = req.Quantity,

                LineSubtotal = lineSubtotal,
                DiscountPercentage = discount,
                LineTotal = lineTotal
            };

            totalAmount += lineTotal;
            await context.OrderItems.AddAsync(orderItem, ct);
        }

        order.TotalAmount = totalAmount;

        // Clear the cart (keep it Active; user continues shopping after checkout)
        context.CartItems.RemoveRange(cart.CartItems);
        cart.TotalAmount = 0m;
        cart.IsEmpty = true;

        await context.SaveChangesAsync(ct);

        await tx.CommitAsync(ct);
        return order.Id;
    }
}