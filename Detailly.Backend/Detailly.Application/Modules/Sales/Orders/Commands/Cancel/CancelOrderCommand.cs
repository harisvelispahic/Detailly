
namespace Detailly.Application.Modules.Sales.Orders.Commands.Cancel;
public class CancelOrderCommand : IRequest<Unit>
{
    public required int Id { get; set; }
}
