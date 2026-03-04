using Detailly.Domain.Common.Enums;

namespace Detailly.Application.Modules.Sales.Orders.Queries.GetMyOrders;

public sealed class GetMyOrdersQuery : BasePagedQuery<GetMyOrdersQueryDto>
{
    public string? Search { get; init; } // optional: search by order number
    public OrderStatus? Status { get; init; } // optional filter
}