using Detailly.Application.Modules.Booking.Locations.Commands.Create;
using Detailly.Application.Modules.Booking.Locations.Commands.Delete;
using Detailly.Application.Modules.Booking.Locations.Commands.Update;
using Detailly.Application.Modules.Booking.Locations.Queries.GetById;
using Detailly.Application.Modules.Booking.Locations.Queries.List;
using Detailly.Shared.Constants;

namespace Detailly.API.Controllers;

[ApiController]
[Route("[controller]")]
public class LocationsController(ISender sender) : ControllerBase
{
    [HttpPost]
    [Authorize(Policy = AuthPolicies.AdminOrManager)]
    public async Task<ActionResult<int>> Create(CreateLocationCommand command, CancellationToken ct)
    {
        int id = await sender.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = AuthPolicies.AdminOrManager)]
    public async Task Update(int id, UpdateLocationCommand command, CancellationToken ct)
    {
        command.Id = id;
        await sender.Send(command, ct);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = AuthPolicies.AdminOrManager)]
    public async Task Delete(int id, CancellationToken ct)
    {
        await sender.Send(new DeleteLocationCommand { Id = id }, ct);
    }

    [HttpGet("{id:int}")]
    [Authorize(Policy = AuthPolicies.AdminOrManager)]
    public async Task<GetLocationByIdQueryDto> GetById(int id, CancellationToken ct)
    {
        return await sender.Send(new GetLocationByIdQuery { Id = id }, ct);
    }

    [HttpGet]
    [Authorize(Policy = AuthPolicies.AdminOrManager)]
    public async Task<List<ListLocationsQueryDto>> List([FromQuery] ListLocationsQuery query, CancellationToken ct)
    {
        return await sender.Send(query, ct);
    }
}