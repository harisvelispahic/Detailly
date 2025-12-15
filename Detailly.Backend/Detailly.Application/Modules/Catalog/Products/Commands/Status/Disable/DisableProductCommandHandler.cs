
namespace Detailly.Application.Modules.Catalog.Products.Commands.Status.Disable;

public sealed class DisableProductCommandHandler(IAppDbContext ctx)
    : IRequestHandler<DisableProductCommand, Unit>
{
    public async Task<Unit> Handle(DisableProductCommand request, CancellationToken ct)
    {
        var pro = await ctx.Products
            .FirstOrDefaultAsync(x => x.Id == request.Id, ct);

        if (pro is null)
        {
            throw new DetaillyNotFoundException($"Product (ID={request.Id}) was not found.");
        }

        if (!pro.IsEnabled) return Unit.Value; // idempotent

        pro.IsEnabled = false;

        await ctx.SaveChangesAsync(ct);

        // await _bus.PublishAsync(new ProductCategoryDisabledV1IntegrationEvent(cat.Id, ...), ct);
        // await _cache.RemoveAsync(CacheKeys.CategoriesList, ct);

        return Unit.Value;
    }
}