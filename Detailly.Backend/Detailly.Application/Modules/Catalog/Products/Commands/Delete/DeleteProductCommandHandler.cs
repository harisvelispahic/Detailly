
namespace Detailly.Application.Modules.Catalog.Products.Commands.Delete;

public class DeleteProductCategoryHandler(IAppDbContext context, IAppCurrentUser appCurrentUser)
      : IRequestHandler<DeleteProductCommand, Unit>
{
    public async Task<Unit> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        if (appCurrentUser.ApplicationUserId is null)
            throw new DetaillyBusinessRuleException("123", "Korisnik nije autentifikovan.");

        var product = await context.Products
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (product is null)
            throw new DetaillyNotFoundException("Proizvod nije pronađen.");

        product.IsDeleted = true; // Soft delete
        await context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
