
namespace Detailly.Application.Modules.Catalog.ProductCategories.Commands.Delete;

public class DeleteProductCategoryCommandHandler(IAppDbContext context, IAppCurrentUser appCurrentUser)
      : IRequestHandler<DeleteProductCategoryCommand, Unit>
{
    public async Task<Unit> Handle(DeleteProductCategoryCommand request, CancellationToken ct)
    {
        if (appCurrentUser.ApplicationUserId is null)
            throw new DetaillyBusinessRuleException("123", "Korisnik nije autentifikovan.");

        var category = await context.ProductCategories
            .FirstOrDefaultAsync(x => x.Id == request.Id, ct);

        if (category is null)
            throw new DetaillyNotFoundException("Product category was not found.");

        category.IsDeleted = true; // Soft delete
        await context.SaveChangesAsync(ct);

        return Unit.Value;
    }
}
