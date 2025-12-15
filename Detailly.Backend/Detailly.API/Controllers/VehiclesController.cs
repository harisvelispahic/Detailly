using Detailly.Application.Modules.Vehicle.Vehicles.Commands.Delete;
using Detailly.Application.Modules.Vehicle.Vehicles.Commands.Create;
using Detailly.Application.Modules.Vehicle.Vehicles.Commands.Update;
using Detailly.Application.Modules.Vehicle.Vehicles.Queries.GetById;
using Detailly.Application.Modules.Vehicle.Vehicles.Queries.List;

namespace Detailly.API.Controllers;

[ApiController]
[Route("[controller]")]
public class VehiclesController(ISender sender) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<int>> Create(CreateVehicleCommand command, CancellationToken ct)
    {
        int id = await sender.Send(command, ct);

        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpPut("{id:int}")]
    public async Task Update(int id, UpdateVehicleCommand command, CancellationToken ct)
    {
        // ID from the route takes precedence
        command.Id = id;
        await sender.Send(command, ct);
        // no return -> 204 No Content
    }

    [HttpDelete("{id:int}")]
    public async Task Delete(int id, CancellationToken ct)
    {
        await sender.Send(new DeleteVehicleCommand { Id = id }, ct);
        // no return -> 204 No Content
    }

    [HttpGet("{id:int}")]
    public async Task<GetVehicleByIdQueryDto> GetById(int id, CancellationToken ct)
    {
        var category = await sender.Send(new GetVehicleByIdQuery { Id = id }, ct);
        return category; // if NotFoundException -> 404 via middleware
    }

    [HttpGet]
    public async Task<List<ListVehiclesQueryDto>> List([FromQuery] ListVehiclesQuery query, CancellationToken ct)
    {
        var result = await sender.Send(query, ct);
        return result;
    }










    //[HttpPut("{id:int}/disable")]
    //public async Task Disable(int id, CancellationToken ct)
    //{
    //    await sender.Send(new DisableProductCategoryCommand { Id = id }, ct);
    //    // no return -> 204 No Content
    //}

    //[HttpPut("{id:int}/enable")]
    //public async Task Enable(int id, CancellationToken ct)
    //{
    //    await sender.Send(new EnableProductCategoryCommand { Id = id }, ct);
    //    // no return -> 204 No Content
    //}
}