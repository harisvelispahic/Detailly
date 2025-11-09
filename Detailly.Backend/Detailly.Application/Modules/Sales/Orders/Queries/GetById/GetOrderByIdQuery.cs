

namespace Detailly.Application.Modules.Sales.Orders.Queries.GetById;
public class GetOrderByIdQuery : IRequest<GetOrderByIdQueryDto>
{
    public int Id { get; set; }
}
