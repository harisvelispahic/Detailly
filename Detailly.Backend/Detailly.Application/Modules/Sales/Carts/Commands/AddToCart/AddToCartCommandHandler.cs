using Detailly.Domain.Common.Enums;
using Detailly.Domain.Entities.Sales;

namespace Detailly.Application.Modules.Sales.Carts.Commands.AddToCart;

public sealed class AddToCartCommandHandler(IAppDbContext context, IAppAuthorizationService authService)
    : IRequestHandler<AddToCartCommand>
{
    public async Task Handle(AddToCartCommand request, CancellationToken ct)
    {
        var userId = authService.RequireUserId();

        if (request.Quantity <= 0)
            throw new DetaillyBusinessRuleException("cart.invalid_quantity", "Quantity must be greater than 0.");

        var product = await context.Products
            .Include(p => p.Inventory)
            .FirstOrDefaultAsync(p => p.Id == request.ProductId, ct);

        if (product is null)
            throw new DetaillyNotFoundException("Product does not exist.");

        if (!product.IsEnabled)
            throw new DetaillyBusinessRuleException("product.disabled", "Product is disabled.");

        // Validate available stock at time of add
        if (product.Inventory.QuantityInStock < request.Quantity)
            throw new DetaillyBusinessRuleException("inventory.insufficient", "Insufficient stock for requested quantity.");

        var cart = await context.Carts
            .Include(c => c.CartItems)
            .FirstOrDefaultAsync(c => c.ApplicationUserId == userId, ct);

        if (cart is null)
        {
            cart = new CartEntity
            {
                ApplicationUserId = userId,
                Status = CartStatus.Active,
                IsEmpty = true,
                TotalAmount = 0m
            };

            context.Carts.Add(cart);
            await context.SaveChangesAsync(ct);
        }

        if (cart.Status != CartStatus.Active)
            throw new DetaillyBusinessRuleException("cart.not_active", "Cart is not active.");

        var existingItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == product.Id);

        // IMPORTANT: total requested qty must also respect stock
        var newQty = (existingItem?.Quantity ?? 0) + request.Quantity;
        if (product.Inventory.QuantityInStock < newQty)
            throw new DetaillyBusinessRuleException("inventory.insufficient", "Insufficient stock for requested quantity.");

        if (existingItem is null)
        {
            var item = new CartItemEntity
            {
                CartId = cart.Id,
                ProductId = product.Id,
                Quantity = request.Quantity,
                UnitPrice = product.Price,
                LineTotal = product.Price * request.Quantity
            };

            context.CartItems.Add(item);
        }
        else
        {
            existingItem.UnitPrice = product.Price; // refresh current price
            existingItem.Quantity = newQty;
            existingItem.LineTotal = existingItem.UnitPrice * existingItem.Quantity;
        }

        // Recalculate cart totals (server-owned)
        await RecalculateAndPersistCartTotals(cart.Id, ct);

        await context.SaveChangesAsync(ct);
    }

    private async Task RecalculateAndPersistCartTotals(int cartId, CancellationToken ct)
    {
        var cart = await context.Carts
            .Include(c => c.CartItems)
            .FirstAsync(c => c.Id == cartId, ct);

        decimal total = 0m;
        foreach (var item in cart.CartItems)
        {
            item.LineTotal = item.UnitPrice * item.Quantity;
            total += item.LineTotal;
        }

        cart.TotalAmount = total;
        cart.IsEmpty = cart.CartItems.Count == 0;
    }
}