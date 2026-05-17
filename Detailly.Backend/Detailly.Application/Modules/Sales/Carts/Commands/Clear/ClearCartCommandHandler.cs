using Detailly.Domain.Common.Enums;
using Detailly.Domain.Entities.Sales;

namespace Detailly.Application.Modules.Sales.Carts.Commands.Clear;

public sealed class ClearCartCommandHandler(IAppDbContext context, IAppCurrentUser appCurrentUser)
    : IRequestHandler<ClearCartCommand>
{
    public async Task Handle(ClearCartCommand request, CancellationToken ct)
    {
        if (!appCurrentUser.IsAuthenticated || appCurrentUser.ApplicationUserId is null)
            throw new DetaillyUnauthorizedException("User is not authenticated.");

        var userId = appCurrentUser.ApplicationUserId.Value;

        var cart = await context.Carts
            .Include(c => c.CartItems)
            .FirstOrDefaultAsync(c => c.ApplicationUserId == userId, ct);

        if (cart is null)
        {
            // create empty cart to keep invariant (one cart per user)
            cart = new CartEntity
            {
                ApplicationUserId = userId,
                Status = CartStatus.Active,
                IsEmpty = true,
                TotalAmount = 0m
            };
            context.Carts.Add(cart);
            await context.SaveChangesAsync(ct);
            return;
        }

        if (cart.Status != CartStatus.Active)
            throw new DetaillyBusinessRuleException("cart.not_active", "Cart is not active.");

        context.CartItems.RemoveRange(cart.CartItems);

        cart.TotalAmount = 0m;
        cart.IsEmpty = true;

        await context.SaveChangesAsync(ct);
    }
}