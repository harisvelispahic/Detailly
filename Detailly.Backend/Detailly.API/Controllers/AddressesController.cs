using Detailly.Application.Modules.Shared.Address.Commands.Create;
using Detailly.Application.Modules.Shared.Address.Commands.Update;
using Detailly.Application.Modules.Shared.Address.Commands.Delete;
using Detailly.Application.Modules.Shared.Address.Queries.GetById;
using Detailly.Application.Modules.Shared.Address.Queries.List;

namespace Detailly.API.Controllers;

[ApiController]
[Route("[controller]")]
public class AddressesController(ISender sender) : ControllerBase
{

    [HttpPost]
    public async Task<ActionResult<int>> Create(
        CreateAddressCommand command,
        CancellationToken ct)
    {
        int id = await sender.Send(command, ct);

        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    
    [HttpPut("{id:int}")]
    public async Task Update(
        int id,
        UpdateAddressCommand command,
        CancellationToken ct)
    {
        // ID from the route takes precedence
        command.Id = id;
        await sender.Send(command, ct);
        // no return -> 204 No Content
    }
    

    [HttpDelete("{id:int}")]
    public async Task Delete(int id, CancellationToken ct)
    {
        await sender.Send(new DeleteAddressCommand { Id = id }, ct);
        // no return -> 204 No Content
    }
    
 
    [HttpGet("{id:int}")]
    public async Task<GetAddressByIdQueryDto> GetById(
        int id,
        CancellationToken ct)
    {
        var address = await sender.Send(
            new GetAddressByIdQuery { Id = id }, ct);

        return address; // NotFoundException -> 404 via middleware
    }
   
   
    [HttpGet]
    public async Task<PageResult<ListAddressesQueryDto>> List(
        [FromQuery] ListAddressesQuery query,
        CancellationToken ct)
    {
        var result = await sender.Send(query, ct);
        return result;
    }
    
}
