namespace Detailly.Application.Modules.Catalog.Products.Commands.Delete;

public class DeleteProductHandler(IAppDbContext context, IAppCurrentUser appCurrentUser)
      : IRequestHandler<DeleteProductCommand, Unit>
{
    public async Task<Unit> Handle(DeleteProductCommand request, CancellationToken ct)
    {
        var now = DateTime.UtcNow;

        if (appCurrentUser.ApplicationUserId is null)
            throw new DetaillyBusinessRuleException("USER_NOT_AUTHENTICATED", "User is not authenticated.");

        var product = await context.Products
            .Include(p => p.Inventory)
            .Include(p => p.Images)
            .FirstOrDefaultAsync(x => x.Id == request.Id, ct);

        if (product is null)
            throw new DetaillyNotFoundException("Product was not found.");

        // Capture affected cart IDs before the items are marked deleted
        var affectedCartIds = await context.CartItems
            .Where(ci => ci.ProductId == request.Id)
            .Select(ci => ci.CartId)
            .Distinct()
            .ToListAsync(ct);

        if (product.Inventory != null)
        {
            product.Inventory.IsDeleted = true;
            product.Inventory.ModifiedAtUtc = now;
        }

        foreach (var image in product.Images)
        {
            image.IsDeleted = true;
            image.ModifiedAtUtc = now;
        }

        var savedProducts = await context.SavedProducts
            .Where(sp => sp.ProductId == request.Id)
            .ToListAsync(ct);

        foreach (var sp in savedProducts)
        {
            sp.IsDeleted = true;
            sp.ModifiedAtUtc = now;
        }

        var cartItems = await context.CartItems
            .Where(ci => ci.ProductId == request.Id)
            .ToListAsync(ct);

        foreach (var ci in cartItems)
        {
            ci.IsDeleted = true;
            ci.ModifiedAtUtc = now;
        }

        product.IsDeleted = true;
        product.ModifiedAtUtc = now;

        await context.SaveChangesAsync(ct);

        // Recalculate totals for carts that lost an item.
        // Must run after SaveChanges so the global filter excludes the now-deleted items.
        if (affectedCartIds.Count > 0)
        {
            var carts = await context.Carts
                .Include(c => c.CartItems)
                .Where(c => affectedCartIds.Contains(c.Id))
                .ToListAsync(ct);

            foreach (var cart in carts)
            {
                cart.TotalAmount = cart.CartItems.Sum(ci => ci.LineTotal);
                cart.IsEmpty = !cart.CartItems.Any();
                cart.ModifiedAtUtc = now;
            }

            await context.SaveChangesAsync(ct);
        }

        return Unit.Value;
    }
}
