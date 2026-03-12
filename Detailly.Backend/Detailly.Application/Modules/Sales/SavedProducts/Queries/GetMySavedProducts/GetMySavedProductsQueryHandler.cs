namespace Detailly.Application.Modules.Sales.SavedProducts.Queries.GetMySavedProducts;

public sealed class GetMySavedProductsQueryHandler(IAppDbContext context, IAppCurrentUser appCurrentUser)
    : IRequestHandler<GetMySavedProductsQuery, List<GetMySavedProductsQueryDto>>
{
    public async Task<List<GetMySavedProductsQueryDto>> Handle(GetMySavedProductsQuery request, CancellationToken ct)
    {
        if (!appCurrentUser.IsAuthenticated || appCurrentUser.ApplicationUserId is null)
            throw new DetaillyUnauthorizedException("User is not authenticated.");

        var userId = appCurrentUser.ApplicationUserId.Value;

        return await context.SavedProducts
            .Where(x => x.ApplicationUserId == userId)
            .Include(x => x.Product)
                .ThenInclude(p => p.Inventory)
            .OrderByDescending(x => x.CreatedAtUtc)
            .Select(x => new GetMySavedProductsQueryDto
            {
                ProductId = x.ProductId,
                ProductName = x.Product.Name,
                Price = x.Product.Price,
                ProductIsEnabled = x.Product.IsEnabled,
                QuantityInStock = x.Product.Inventory.QuantityInStock,
                CreatedAtUtc = x.CreatedAtUtc
            })
            .ToListAsync(ct);
    }
}