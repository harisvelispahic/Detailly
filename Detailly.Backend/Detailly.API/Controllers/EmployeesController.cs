using Detailly.Application.Modules.Identity.Employee.Queries.List;
using Detailly.Domain.Common.Enums;
using Detailly.Shared.Constants;

namespace Detailly.API.Controllers;

[ApiController]
[Route("[controller]")]
public sealed class EmployeesController(ISender sender) : ControllerBase
{
    // GET /Employees?employeeWorkMode=0
    [HttpGet]
    [Authorize(Policy = AuthPolicies.AdminOrManager)]
    public async Task<List<ListEmployeesQueryDto>> List([FromQuery] EmployeeWorkMode? employeeWorkMode, CancellationToken ct)
    {
        return await sender.Send(new ListEmployeesQuery { EmployeeWorkMode = employeeWorkMode }, ct);
    }
}
