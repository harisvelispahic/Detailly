
namespace Detailly.Application.Modules.Catalog.Products.Queries.List;

public sealed class ListProductsQuery : BasePagedQuery<ListProductsQueryDto>
{
    public string? Search { get; init; }
    public bool? OnlyEnabled { get; init; }
}
