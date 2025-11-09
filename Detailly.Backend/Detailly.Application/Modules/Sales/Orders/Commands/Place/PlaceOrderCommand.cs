
namespace Detailly.Application.Modules.Sales.Orders.Commands.Place;
public class PlaceOrderCommand : IRequest<Unit>
{
    public required int Id { get; set; }
}
