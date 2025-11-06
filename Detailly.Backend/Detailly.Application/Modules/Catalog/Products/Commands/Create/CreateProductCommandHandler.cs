
using Detailly.Domain.Common.Enums;

namespace Detailly.Application.Modules.Catalog.Products.Commands.Create;

public class CreateProductCommandHandler(IAppDbContext context)
    : IRequestHandler<CreateProductCommand, int>
{
    public async Task<int> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var normalized = request.Name?.Trim();
        if (string.IsNullOrWhiteSpace(normalized))
            throw new ValidationException("Name is required.");

        if (string.IsNullOrWhiteSpace(request.Description?.Trim()))
            throw new ValidationException("Description is required.");

        if (request.Price == null || request.Price <= 0)
            throw new ValidationException("Price is required.");

        if (request.CategoryId == null || request.CategoryId <= 0)
            throw new ValidationException("Category is required.");

        // Check if a category with the same name already exists.
        bool exists = await context.Products
            .AnyAsync(x => x.Name == normalized, cancellationToken);

        if (exists)
        {
            throw new DetaillyConflictException("Name already exists.");
        }

        var product = new ProductEntity
        {
            Name = request.Name!.Trim(),
            Description = request.Description!.Trim(),
            Price = request.Price,
            ProductNumber = Guid.NewGuid().ToString(),
            CategoryId= request.CategoryId,
            Currency = request.Currency ?? CurrencyName.BAM
            //IsEnabled = true, // deault IsEnabled
        };

        context.Products.Add(product);
        await context.SaveChangesAsync(cancellationToken);

        return product.Id;
    }
}