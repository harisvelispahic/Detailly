using Detailly.Application.Modules.Booking.ServicePackages.Commands.Create;
using Detailly.Application.Modules.Booking.ServicePackages.Commands.Delete;
using Detailly.Application.Modules.Booking.ServicePackages.Commands.Update;
using Detailly.Application.Modules.Booking.ServicePackages.Queries.GetById;
using Detailly.Application.Modules.Booking.ServicePackages.Queries.List;

namespace Detailly.API.Controllers;

[ApiController]
[Route("[controller]")]
public class ServicePackagesController(ISender sender) : ControllerBase
{
    [Authorize]
    [HttpPost]
    public async Task<ActionResult<int>> Create(CreateServicePackageCommand command, CancellationToken ct)
    {
        int id = await sender.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [Authorize]
    [HttpPut("{id:int}")]
    public async Task Update(int id, UpdateServicePackageCommand command, CancellationToken ct)
    {
        command.Id = id; // ID from route takes precedence
        await sender.Send(command, ct);
    }

    [Authorize]
    [HttpDelete("{id:int}")]
    public async Task Delete(int id, CancellationToken ct)
    {
        await sender.Send(new DeleteServicePackageCommand { Id = id }, ct);
    }

    [HttpGet("{id:int}")]
    public async Task<GetServicePackageByIdQueryDto> GetById(int id, CancellationToken ct)
    {
        var sp = await sender.Send(new GetServicePackageByIdQuery { Id = id }, ct);
        return sp;
    }

    [HttpGet]
    public async Task<PageResult<ListServicePackagesQueryDto>> List([FromQuery] ListServicePackagesQuery query, CancellationToken ct)
    {
        var result = await sender.Send(query, ct);
        return result;
    }
}
