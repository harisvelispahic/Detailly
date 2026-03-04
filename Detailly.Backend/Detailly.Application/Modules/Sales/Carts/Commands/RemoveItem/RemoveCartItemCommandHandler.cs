using Detailly.Domain.Common.Enums;

namespace Detailly.Application.Modules.Sales.Carts.Commands.RemoveItem;

public sealed class RemoveCartItemCommandHandler(IAppDbContext context, IAppCurrentUser appCurrentUser)
    : IRequestHandler<RemoveCartItemCommand>
{
    public async Task Handle(RemoveCartItemCommand request, CancellationToken ct)
    {
        if (!appCurrentUser.IsAuthenticated || appCurrentUser.ApplicationUserId is null)
            throw new UnauthorizedAccessException("User is not authenticated.");

        var userId = appCurrentUser.ApplicationUserId.Value;

        var cartItem = await context.CartItems
            .Include(ci => ci.Cart)
            .FirstOrDefaultAsync(ci => ci.Id == request.CartItemId, ct);

        if (cartItem is null)
            throw new DetaillyNotFoundException("Cart item was not found.");

        if (cartItem.Cart.ApplicationUserId != userId)
            throw new UnauthorizedAccessException("You do not have access to this cart.");

        if (cartItem.Cart.Status != CartStatus.Active)
            throw new InvalidOperationException("Cart is not active.");

        context.CartItems.Remove(cartItem);

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