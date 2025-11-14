
namespace Detailly.Application.Modules.Catalog.Products.Commands.Delete;

public class DeleteProductHandler(IAppDbContext context, IAppCurrentUser appCurrentUser)
      : IRequestHandler<DeleteProductCommand, Unit>
{
    public async Task<Unit> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        if (appCurrentUser.ApplicationUserId is null)
            throw new DetaillyBusinessRuleException("123", "Korisnik nije autentifikovan.");

        var product = await context.Products
            .Include(p => p.Inventory)
            .Include(p => p.Images)
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (product is null)
            throw new DetaillyNotFoundException("Proizvod nije pronađen.");

        product.IsDeleted = true; // Soft delete

        // mark related entities
        if (product.Inventory != null)
        {
            product.Inventory.IsDeleted = true;
            product.Inventory.ModifiedAtUtc = DateTime.UtcNow;
        }

        foreach (var image in product.Images)
        {
            image.IsDeleted = true;
            image.ModifiedAtUtc = DateTime.UtcNow;
        }

        await context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
