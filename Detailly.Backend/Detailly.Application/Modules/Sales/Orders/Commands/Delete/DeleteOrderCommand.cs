
namespace Detailly.Application.Modules.Sales.Orders.Commands.Delete;
public class DeleteOrderCommand : IRequest<Unit>
{
    public required int Id { get; set; }
}
