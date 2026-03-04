namespace Detailly.Application.Modules.Sales.Carts.Commands.RemoveItem;

public sealed class RemoveCartItemCommand : IRequest
{
    public int CartItemId { get; set; }
}