using Detailly.Application.Modules.Shared.Address.Commands.Create;
using Detailly.Application.Modules.Shared.Address.Commands.Update;
using Detailly.Application.Modules.Shared.Address.Commands.Delete;
using Detailly.Application.Modules.Shared.Address.Queries.GetById;
using Detailly.Application.Modules.Shared.Address.Queries.List;
using Detailly.Shared.Constants;
using Detailly.Application.Modules.Shared.Address.Queries.ListMine;

namespace Detailly.API.Controllers;

[ApiController]
[Route("[controller]")]
public class AddressesController(ISender sender) : ControllerBase
{
    [HttpPost]
    [Authorize(Policy = AuthPolicies.AnyClient)]
    public async Task<ActionResult<int>> Create(
        CreateAddressCommand command,
        CancellationToken ct)
    {
        int id = await sender.Send(command, ct);

        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = AuthPolicies.AnyClient)]
    public async Task<IActionResult> Update(
        int id,
        UpdateAddressCommand command,
        CancellationToken ct)
    {
        command.Id = id;
        await sender.Send(command, ct);

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = AuthPolicies.AnyClient)]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await sender.Send(new DeleteAddressCommand { Id = id }, ct);

        return NoContent();
    }

    [HttpGet("{id:int}")]
    [Authorize(Policy = AuthPolicies.AnyClient)]
    public async Task<ActionResult<GetAddressByIdQueryDto>> GetById(
        int id,
        CancellationToken ct)
    {
        var address = await sender.Send(
            new GetAddressByIdQuery { Id = id }, ct);

        return Ok(address);
    }

    [HttpGet]
    [Authorize(Policy = AuthPolicies.AdminOrManager)]
    public async Task<ActionResult<PageResult<ListAddressesQueryDto>>> List(
        [FromQuery] ListAddressesQuery query,
        CancellationToken ct)
    {
        var result = await sender.Send(query, ct);
        return Ok(result);
    }

    [HttpGet("my")]
    [Authorize(Policy = AuthPolicies.AnyClient)]
    public async Task<ActionResult<PageResult<ListMyAddressesQueryDto>>> ListMine(
        [FromQuery] ListMyAddressesQuery query,
        CancellationToken ct)
    {
        var result = await sender.Send(query, ct);
        return Ok(result);
    }
}