namespace Detailly.Application.Modules.Catalog.Products.Commands.Status.Enable;

public sealed class EnableProductCommandHandler(IAppDbContext ctx)
    : IRequestHandler<EnableProductCommand, Unit>
{
    public async Task<Unit> Handle(EnableProductCommand request, CancellationToken ct)
    {
        var product = await ctx.Products
            .FirstOrDefaultAsync(x => x.Id == request.Id, ct);

        if (product is null)
            throw new DetaillyNotFoundException($"Product (ID={request.Id}) was not found.");

        if (!product.IsEnabled)
        {
            product.IsEnabled = true;
            await ctx.SaveChangesAsync(ct);
        }

        // If already enabled — nothing changes, idempotent
        return Unit.Value;
    }
}
