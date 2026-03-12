namespace Detailly.Application.Modules.Catalog.ProductCategories.Commands.Patch;

public sealed class PatchProductCategoryCommandHandler(IAppDbContext context)
    : IRequestHandler<PatchProductCategoryCommand, Unit>
{
    public async Task<Unit> Handle(PatchProductCategoryCommand request, CancellationToken ct)
    {
        var entity = await context.ProductCategories
            .FirstOrDefaultAsync(x => x.Id == request.Id, ct);

        if (entity is null || entity.IsDeleted)
            throw new DetaillyNotFoundException($"Product category (ID={request.Id}) was not found.");

        if (request.Name is not null)
            entity.Name = request.Name.Trim();

        if (request.Description is not null)
            entity.Description = request.Description.Trim();

        await context.SaveChangesAsync(ct);

        return Unit.Value;
    }
}