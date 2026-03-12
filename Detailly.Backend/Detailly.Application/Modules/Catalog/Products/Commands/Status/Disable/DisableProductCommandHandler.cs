namespace Detailly.Application.Modules.Catalog.Products.Commands.Status.Disable;

public sealed class DisableProductCommandHandler(IAppDbContext ctx)
    : IRequestHandler<DisableProductCommand, Unit>
{
    public async Task<Unit> Handle(DisableProductCommand request, CancellationToken ct)
    {
        var product = await ctx.Products
            .FirstOrDefaultAsync(x => x.Id == request.Id, ct);

        if (product is null)
        {
            throw new DetaillyNotFoundException($"Product (ID={request.Id}) was not found.");
        }

        if (!product.IsEnabled) return Unit.Value; // idempotent

        product.IsEnabled = false;

        await ctx.SaveChangesAsync(ct);

        // await _bus.PublishAsync(new ProductCategoryDisabledV1IntegrationEvent(cat.Id, ...), ct);
        // await _cache.RemoveAsync(CacheKeys.CategoriesList, ct);

        return Unit.Value;
    }
}