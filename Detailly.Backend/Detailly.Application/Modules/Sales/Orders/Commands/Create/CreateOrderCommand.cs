
using Detailly.Domain.Common.Enums;

namespace Detailly.Application.Modules.Sales.Orders.Commands.Create;

public class CreateOrderCommand : IRequest<int>
{
    public required int ShipToAddressId { get; set; }
    public string? Notes { get; set; }

    public required List<CreateOrderCommandOrderItems> OrderItems { get; set; }
}

public class CreateOrderCommandOrderItems
{
    //public required int OrderId { get; set; }
    public required int ProductId { get; set; }
    public required CurrencyName Currency { get; set; }
    public required decimal UnitPrice { get; set; }
    public required int Quantity { get; set; }

}
