using Detailly.Application.Modules.Sales.Orders.Commands.Cancel;
using Detailly.Application.Modules.Sales.Orders.Commands.CheckoutCart;
using Detailly.Application.Modules.Sales.Orders.Commands.MarkDelivered;
using Detailly.Application.Modules.Sales.Orders.Commands.MarkShipped;
using Detailly.Application.Modules.Sales.Orders.Queries.GetById;
using Detailly.Application.Modules.Sales.Orders.Queries.GetMyOrders;
using Detailly.Application.Modules.Sales.Orders.Queries.GetOrderDetails;
using Detailly.Application.Modules.Sales.Orders.Queries.List;
using Detailly.Shared.Constants;

namespace Detailly.API.Controllers;

[ApiController]
[Route("[controller]")]
public class OrdersController(ISender sender) : ControllerBase
{
    // Same thing, explicit "checkout" route
    [HttpPost("checkout")]
    [Authorize(Policy = AuthPolicies.AnyClient)]
    public async Task<ActionResult<int>> Checkout([FromBody] CheckoutCartCommand command, CancellationToken ct)
    {
        int id = await sender.Send(command, ct);
        return CreatedAtAction(nameof(GetOrderDetails), new { id }, new { id });
    }

    [HttpPut("cancel/{id:int}")]
    [Authorize(Policy = AuthPolicies.AnyClient)]
    public async Task Cancel(int id, [FromBody] CancelOrderCommand command, CancellationToken ct)
    {
        command.Id = id;
        await sender.Send(command, ct);
    }

    // My order history
    [HttpGet("my")]
    [Authorize(Policy = AuthPolicies.AnyClient)]
    public async Task<PageResult<GetMyOrdersQueryDto>> GetMy([FromQuery] GetMyOrdersQuery query, CancellationToken ct)
        => await sender.Send(query, ct);

    // Ownership-enforced details
    [HttpGet("details/{id:int}")]
    [Authorize(Policy = AuthPolicies.AnyClient)]
    public async Task<GetOrderDetailsQueryDto> GetOrderDetails(int id, CancellationToken ct)
        => await sender.Send(new GetOrderDetailsQuery { Id = id }, ct);

    // Keep existing list/get endpoints (staff list)
    [HttpGet("{id:int}")]
    [Authorize(Policy = AuthPolicies.AnyClient)]
    public async Task<GetOrderByIdQueryDto> GetById(int id, CancellationToken ct)
        => await sender.Send(new GetOrderByIdQuery { Id = id }, ct);

    [HttpGet]
    [Authorize(Policy = AuthPolicies.Staff)]
    public async Task<PageResult<ListOrdersQueryDto>> List([FromQuery] ListOrdersQuery query, CancellationToken ct)
        => await sender.Send(query, ct);

    [HttpPut("ship/{id:int}")]
    [Authorize(Policy = AuthPolicies.Staff)]
    public async Task Ship(int id, [FromBody] MarkOrderShippedCommand command, CancellationToken ct)
    {
        command.Id = id;
        await sender.Send(command, ct);
    }

    [HttpPut("deliver/{id:int}")]
    [Authorize(Policy = AuthPolicies.Staff)]
    public async Task Deliver(int id, [FromBody] MarkOrderDeliveredCommand command, CancellationToken ct)
    {
        command.Id = id;
        await sender.Send(command, ct);
    }
}