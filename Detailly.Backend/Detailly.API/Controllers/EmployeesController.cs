using Detailly.Application.Common;
using Detailly.Application.Modules.Identity.Employee.Queries.List;
using Detailly.Shared.Constants;

namespace Detailly.API.Controllers;

[ApiController]
[Route("[controller]")]
public sealed class EmployeesController(ISender sender) : ControllerBase
{
    // GET /Employees
    [HttpGet]
    [Authorize(Policy = AuthPolicies.AdminOrManager)]
    public async Task<PageResult<ListEmployeesQueryDto>> List([FromQuery] ListEmployeesQuery query, CancellationToken ct)
    {
        return await sender.Send(query, ct);
    }
}
