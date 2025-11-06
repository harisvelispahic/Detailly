
namespace Detailly.Application.Modules.Catalog.Products.Commands.Status.Enable;

public sealed class EnableProductCommandHandler(IAppDbContext ctx)
    : IRequestHandler<EnableProductCommand, Unit>
{
    public async Task<Unit> Handle(EnableProductCommand request, CancellationToken ct)
    {
        var entity = await ctx.Products
            .FirstOrDefaultAsync(x => x.Id == request.Id, ct);

        if (entity is null)
            throw new DetaillyNotFoundException($"Proizvod (ID={request.Id}) nije pronađen.");

        if (!entity.IsEnabled)
        {
            entity.IsEnabled = true;
            await ctx.SaveChangesAsync(ct);
        }

        // If already enabled — nothing changes, idempotent
        return Unit.Value;
    }
}
