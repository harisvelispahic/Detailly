
namespace Detailly.Application.Modules.Sales.Orders.Queries.List;
public class ListOrdersQuery : BasePagedQuery<ListOrdersQueryDto>
{
    public string? Search { get; init; }
}
