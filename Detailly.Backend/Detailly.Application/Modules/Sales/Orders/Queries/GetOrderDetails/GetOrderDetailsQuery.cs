namespace Detailly.Application.Modules.Sales.Orders.Queries.GetOrderDetails;

public sealed class GetOrderDetailsQuery : IRequest<GetOrderDetailsQueryDto>
{
    public int Id { get; set; }
}