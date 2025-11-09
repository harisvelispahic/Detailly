
namespace Detailly.Application.Modules.Sales.Orders.Commands.Update;
public class UpdateOrderCommand : IRequest<int>
{
    public required int Id { get; set; }
    public string? Notes { get; set; }
    public int? ShipToAddressId { get; set; }

}
