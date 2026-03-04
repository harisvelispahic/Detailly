namespace Detailly.Application.Modules.Sales.Carts.Commands.UpdateItemQuantity;

public sealed class UpdateCartItemQuantityCommand : IRequest
{
    public int CartItemId { get; set; }
    public int Quantity { get; set; }
}