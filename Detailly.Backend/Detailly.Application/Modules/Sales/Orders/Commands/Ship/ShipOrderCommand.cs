
namespace Detailly.Application.Modules.Sales.Orders.Commands.Ship;
public class ShipOrderCommand : IRequest<Unit>
{
    public required int Id { get; set; }
}
