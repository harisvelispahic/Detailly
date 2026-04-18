namespace Detailly.Application.Modules.Sales.SavedProducts.Queries.ListMySavedProducts;

public sealed class ListMySavedProductsQueryHandler(IAppDbContext context, IAppCurrentUser appCurrentUser)
    : IRequestHandler<ListMySavedProductsQuery, PageResult<ListMySavedProductsQueryDto>>
{
    public async Task<PageResult<ListMySavedProductsQueryDto>> Handle(ListMySavedProductsQuery request, CancellationToken ct)
    {
        if (!appCurrentUser.IsAuthenticated || appCurrentUser.ApplicationUserId is null)
            throw new DetaillyUnauthorizedException("User is not authenticated.");

        var q = context.SavedProducts.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var s = request.Search.Trim();
            q = q.Where(x => x.Product.Name.Contains(s));
        }

        var projectedQuery = q
            .Where(x => x.ApplicationUserId == appCurrentUser.ApplicationUserId.Value)
            .Include(x => x.Product)
                .ThenInclude(p => p.Inventory)
            .OrderByDescending(x => x.CreatedAtUtc)
            .Select(x => new ListMySavedProductsQueryDto
            {
                ProductId = x.ProductId,
                ProductName = x.Product.Name,
                Price = x.Product.Price,
                ProductIsEnabled = x.Product.IsEnabled,
                QuantityInStock = x.Product.Inventory.QuantityInStock,
                CreatedAtUtc = x.CreatedAtUtc
            });

        return await PageResult<ListMySavedProductsQueryDto>.FromQueryableAsync(projectedQuery, request.Paging, ct);
    }
}
