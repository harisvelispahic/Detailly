using Detailly.Domain.Common.Enums;

namespace Detailly.Application.Modules.Sales.Carts.Commands.UpdateItemQuantity;

public sealed class UpdateCartItemQuantityCommandHandler(IAppDbContext context, IAppCurrentUser appCurrentUser)
    : IRequestHandler<UpdateCartItemQuantityCommand>
{
    public async Task Handle(UpdateCartItemQuantityCommand request, CancellationToken ct)
    {
        if (!appCurrentUser.IsAuthenticated || appCurrentUser.ApplicationUserId is null)
            throw new UnauthorizedAccessException("User is not authenticated.");

        var userId = appCurrentUser.ApplicationUserId.Value;

        var cartItem = await context.CartItems
            .Include(ci => ci.Cart)
            .Include(ci => ci.Product)
                .ThenInclude(p => p.Inventory)
            .FirstOrDefaultAsync(ci => ci.Id == request.CartItemId, ct);

        if (cartItem is null)
            throw new DetaillyNotFoundException("Cart item was not found.");

        if (cartItem.Cart.ApplicationUserId != userId)
            throw new UnauthorizedAccessException("You do not have access to this cart.");

        if (cartItem.Cart.Status != CartStatus.Active)
            throw new InvalidOperationException("Cart is not active.");

        if (!cartItem.Product.IsEnabled)
            throw new InvalidOperationException("Product is disabled.");

        // Quantity 0 = remove item (nice UX)
        if (request.Quantity <= 0)
        {
            context.CartItems.Remove(cartItem);
            await RecalculateCartTotals(cartItem.CartId, ct);
            await context.SaveChangesAsync(ct);
            return;
        }

        if (cartItem.Product.Inventory.QuantityInStock < request.Quantity)
            throw new InvalidOperationException("Insufficient stock for requested quantity.");

        // Refresh current price (optional but common)
        cartItem.UnitPrice = cartItem.Product.Price;
        cartItem.Quantity = request.Quantity;
        cartItem.LineTotal = cartItem.UnitPrice * cartItem.Quantity;

        await RecalculateCartTotals(cartItem.CartId, ct);
        await context.SaveChangesAsync(ct);
    }

    private async Task RecalculateCartTotals(int cartId, CancellationToken ct)
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