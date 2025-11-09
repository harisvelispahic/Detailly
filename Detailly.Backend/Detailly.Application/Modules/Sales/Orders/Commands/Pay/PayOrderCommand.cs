
namespace Detailly.Application.Modules.Sales.Orders.Commands.Pay;
public class PayOrderCommand : IRequest<Unit>
{
    public required int Id { get; set; }
}
