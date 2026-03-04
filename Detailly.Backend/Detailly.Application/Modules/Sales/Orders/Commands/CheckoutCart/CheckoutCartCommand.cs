namespace Detailly.Application.Modules.Sales.Orders.Commands.CheckoutCart;

public sealed class CheckoutCartCommand : IRequest<int>
{
    public int ShipToAddressId { get; set; }
    public string? Notes { get; set; }
}