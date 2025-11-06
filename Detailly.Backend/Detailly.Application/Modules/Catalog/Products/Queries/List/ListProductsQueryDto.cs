
namespace Detailly.Application.Modules.Catalog.Products.Queries.List;

public sealed class ListProductsQueryDto
{
    public required int Id { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required decimal Price { get; init; }
    public required bool IsEnabled { get; init; }
}
