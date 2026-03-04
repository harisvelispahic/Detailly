namespace Detailly.Application.Modules.Sales.Carts.Commands.AddToCart;

public sealed class AddToCartCommand : IRequest
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}