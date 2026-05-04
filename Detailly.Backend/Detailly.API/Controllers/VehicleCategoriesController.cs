using Detailly.Application.Common;
using Detailly.Application.Modules.Vehicle.VehicleCategories.Commands.Create;
using Detailly.Application.Modules.Vehicle.VehicleCategories.Commands.Delete;
using Detailly.Application.Modules.Vehicle.VehicleCategories.Commands.Update;
using Detailly.Application.Modules.Vehicle.VehicleCategories.Queries.GetById;
using Detailly.Application.Modules.Vehicle.VehicleCategories.Queries.List;
using Detailly.Shared.Constants;

namespace Detailly.API.Controllers;

[ApiController]
[Route("[controller]")]
public class VehicleCategoriesController(ISender sender) : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<PageResult<ListVehicleCategoriesQueryDto>> List([FromQuery] ListVehicleCategoriesQuery query, CancellationToken ct)
    {
        return await sender.Send(query, ct);
    }

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<GetVehicleCategoryByIdQueryDto> GetById(int id, CancellationToken ct)
    {
        return await sender.Send(new GetVehicleCategoryByIdQuery { Id = id }, ct);
    }

    [HttpPost]
    [Authorize(Policy = AuthPolicies.AdminOrManager)]
    public async Task<ActionResult<int>> Create(CreateVehicleCategoryCommand command, CancellationToken ct)
    {
        int id = await sender.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = AuthPolicies.AdminOrManager)]
    public async Task Update(int id, UpdateVehicleCategoryCommand command, CancellationToken ct)
    {
        command.Id = id;
        await sender.Send(command, ct);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = AuthPolicies.AdminOrManager)]
    public async Task Delete(int id, CancellationToken ct)
    {
        await sender.Send(new DeleteVehicleCategoryCommand { Id = id }, ct);
    }
}
