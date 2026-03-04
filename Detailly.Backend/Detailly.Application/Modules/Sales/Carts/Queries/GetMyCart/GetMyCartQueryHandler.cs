using Detailly.Domain.Common.Enums;
using Detailly.Domain.Entities.Sales;

namespace Detailly.Application.Modules.Sales.Carts.Queries.GetMyCart;

public sealed class GetMyCartQueryHandler(IAppDbContext context, IAppCurrentUser appCurrentUser)
    : IRequestHandler<GetMyCartQuery, GetMyCartQueryDto>
{
    public async Task<GetMyCartQueryDto> Handle(GetMyCartQuery request, CancellationToken ct)
    {
        if (!appCurrentUser.IsAuthenticated || appCurrentUser.ApplicationUserId is null)
            throw new UnauthorizedAccessException("User is not authenticated.");

        var userId = appCurrentUser.ApplicationUserId.Value;

        var cart = await context.Carts
            .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                    .ThenInclude(p => p.Inventory)
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

            return new GetMyCartQueryDto
            {
                CartId = cart.Id,
                TotalAmount = 0m,
                IsEmpty = true,
                Status = cart.Status.ToString(),
                Items = new()
            };
        }

        // Ensure totals are consistent (server-owned)
        RecalculateCartTotals(cart);

        await context.SaveChangesAsync(ct);

        return new GetMyCartQueryDto
        {
            CartId = cart.Id,
            TotalAmount = cart.TotalAmount,
            IsEmpty = cart.IsEmpty,
            Status = cart.Status.ToString(),
            Items = cart.CartItems
                .OrderByDescending(x => x.CreatedAtUtc)
                .Select(ci => new GetMyCartItemQueryDto
                {
                    CartItemId = ci.Id,
                    ProductId = ci.ProductId,
                    ProductName = ci.Product.Name,
                    UnitPrice = ci.UnitPrice,
                    Quantity = ci.Quantity,
                    LineTotal = ci.LineTotal,
                    ProductIsEnabled = ci.Product.IsEnabled,
                    QuantityInStock = ci.Product.Inventory.QuantityInStock
                })
                .ToList()
        };
    }

    private static void RecalculateCartTotals(CartEntity cart)
    {
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