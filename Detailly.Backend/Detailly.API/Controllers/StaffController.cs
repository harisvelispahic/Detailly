using Detailly.Application.Modules.Identity.Staff.Commands.Create;
using Detailly.Application.Modules.Identity.Staff.Commands.Update;
using Detailly.Application.Modules.Identity.Staff.Commands.Delete;
using Detailly.Application.Modules.Identity.Staff.Queries.GetById;
using Detailly.Application.Modules.Identity.Staff.Queries.List;
using Detailly.Shared.Constants;

namespace Detailly.API.Controllers;

[ApiController]
[Route("[controller]")]
public sealed class StaffController(ISender sender) : ControllerBase
{
    [HttpPost]
    [Authorize(Policy = AuthPolicies.AdminOrManager)]
    public async Task<ActionResult<int>> Create(CreateStaffMemberCommand command, CancellationToken ct)
    {
        int id = await sender.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = AuthPolicies.AdminOrManager)]
    public async Task Update(int id, UpdateStaffMemberCommand command, CancellationToken ct)
    {
        command.Id = id;
        await sender.Send(command, ct);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = AuthPolicies.AdminOrManager)]
    public async Task Delete(int id, CancellationToken ct)
    {
        await sender.Send(new DeleteStaffMemberCommand { Id = id }, ct);
    }

    [HttpGet("{id:int}")]
    [Authorize(Policy = AuthPolicies.AdminOrManager)]
    public async Task<GetStaffMemberByIdQueryDto> GetById(int id, CancellationToken ct)
    {
        return await sender.Send(new GetStaffMemberByIdQuery { Id = id }, ct);
    }

    [HttpGet]
    [Authorize(Policy = AuthPolicies.AdminOrManager)]
    public async Task<PageResult<ListStaffMembersQueryDto>> List([FromQuery] ListStaffMembersQuery query, CancellationToken ct)
    {
        return await sender.Send(query, ct);
    }
}
