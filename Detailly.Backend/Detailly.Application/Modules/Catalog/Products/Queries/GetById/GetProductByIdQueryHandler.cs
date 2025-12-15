
namespace Detailly.Application.Modules.Catalog.Products.Queries.GetById;

public class GetProductByIdQueryHandler(IAppDbContext context) : IRequestHandler<GetProductByIdQuery, GetProductByIdQueryDto>
{
    public async Task<GetProductByIdQueryDto> Handle(GetProductByIdQuery request, CancellationToken ct)
    {
        var product = await context.Products
            .Where(c => c.Id == request.Id)
            .Select(x => new GetProductByIdQueryDto
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                Price = x.Price,
                IsEnabled = x.IsEnabled
            })
            .FirstOrDefaultAsync(ct);

        if (product == null)
        {
            throw new DetaillyNotFoundException($"Product with (ID = {request.Id}) not found.");
        }

        return product;
    }
}