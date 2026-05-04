using Detailly.Application.Modules.Booking.ServicePackageItems.Commands.Create;
using Detailly.Application.Modules.Booking.ServicePackageItems.Commands.Delete;
using Detailly.Application.Modules.Booking.ServicePackageItems.Commands.Update;
using Detailly.Application.Modules.Booking.ServicePackageItems.Queries.GetById;
using Detailly.Application.Modules.Booking.ServicePackageItems.Queries.List;
using Detailly.Shared.Constants;

namespace Detailly.API.Controllers;

[ApiController]
[Route("[controller]")]
public class ServicePackageItemsController(ISender sender) : ControllerBase
{
    [HttpGet]
    [Authorize(Policy = AuthPolicies.AdminOrManager)]
    public async Task<PageResult<ListServicePackageItemsQueryDto>> List(
        [FromQuery] ListServicePackageItemsQuery query, CancellationToken ct)
    {
        return await sender.Send(query, ct);
    }

    [HttpGet("{id:int}")]
    [Authorize(Policy = AuthPolicies.AdminOrManager)]
    public async Task<GetServicePackageItemByIdQueryDto> GetById(int id, CancellationToken ct)
    {
        return await sender.Send(new GetServicePackageItemByIdQuery { Id = id }, ct);
    }

    [HttpPost]
    [Authorize(Policy = AuthPolicies.AdminOrManager)]
    public async Task<IActionResult> Create([FromBody] CreateServicePackageItemCommand command, CancellationToken ct)
    {
        var id = await sender.Send(command, ct);
        return Ok(new { id });
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = AuthPolicies.AdminOrManager)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateServicePackageItemCommand command, CancellationToken ct)
    {
        command.Id = id;
        await sender.Send(command, ct);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = AuthPolicies.AdminOrManager)]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await sender.Send(new DeleteServicePackageItemCommand { Id = id }, ct);
        return NoContent();
    }
}
