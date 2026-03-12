namespace Detailly.Application.Modules.Catalog.ProductCategories.Commands.Update;

public sealed class UpdateProductCategoryCommandHandler(IAppDbContext context)
    : IRequestHandler<UpdateProductCategoryCommand, Unit>
{
    public async Task<Unit> Handle(UpdateProductCategoryCommand request, CancellationToken ct)
    {
        var entity = await context.ProductCategories
            .FirstOrDefaultAsync(x => x.Id == request.Id, ct);

        if (entity is null || entity.IsDeleted)
            throw new DetaillyNotFoundException($"Product category (ID={request.Id}) was not found.");

        entity.Name = request.Name.Trim();
        entity.Description = request.Description?.Trim();

        await context.SaveChangesAsync(ct);

        return Unit.Value;
    }
}