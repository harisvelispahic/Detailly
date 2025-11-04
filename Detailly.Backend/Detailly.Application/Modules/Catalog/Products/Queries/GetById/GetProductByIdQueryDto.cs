
namespace Detailly.Application.Modules.Catalog.Products.Queries.GetById;

public class GetProductByIdQueryDto
{
    public required int Id { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required decimal Price { get; init; }
    public required bool IsEnabled { get; init; }
}
