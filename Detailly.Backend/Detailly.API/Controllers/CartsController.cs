using Detailly.Application.Modules.Sales.Carts.Commands.AddToCart;
using Detailly.Application.Modules.Sales.Carts.Commands.Clear;
using Detailly.Application.Modules.Sales.Carts.Commands.RemoveItem;
using Detailly.Application.Modules.Sales.Carts.Commands.UpdateItemQuantity;
using Detailly.Application.Modules.Sales.Carts.Queries.GetMyCart;
using Detailly.Shared.Constants;

namespace Detailly.API.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize(Policy = AuthPolicies.AnyClient)]
public sealed class CartsController(ISender sender) : ControllerBase
{
    [HttpGet("my")]
    public async Task<GetMyCartQueryDto> GetMy(CancellationToken ct)
        => await sender.Send(new GetMyCartQuery(), ct);

    [HttpPost("items")]
    public async Task AddItem([FromBody] AddToCartCommand command, CancellationToken ct)
        => await sender.Send(command, ct);

    [HttpPut("items/{cartItemId:int}")]
    public async Task UpdateItemQuantity(int cartItemId, [FromBody] UpdateCartItemQuantityCommand command, CancellationToken ct)
    {
        command.CartItemId = cartItemId;
        await sender.Send(command, ct);
    }

    [HttpDelete("items/{cartItemId:int}")]
    public async Task RemoveItem(int cartItemId, CancellationToken ct)
        => await sender.Send(new RemoveCartItemCommand { CartItemId = cartItemId }, ct);

    [HttpDelete("clear")]
    public async Task Clear(CancellationToken ct)
        => await sender.Send(new ClearCartCommand(), ct);
}