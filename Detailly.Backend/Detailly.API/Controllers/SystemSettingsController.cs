using Detailly.Application.Modules.Settings;
using Detailly.Application.Modules.Settings.Commands.UpdateSystemSettings;
using Detailly.Application.Modules.Settings.Queries.GetSystemSettings;
using Detailly.Shared.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Detailly.API.Controllers;

[ApiController]
[Route("[controller]")]
public class SystemSettingsController(ISender sender) : ControllerBase
{
    [HttpGet]
    [Authorize(Policy = AuthPolicies.AdminOnly)]
    public async Task<SystemSettingsDto> Get(CancellationToken ct)
    {
        return await sender.Send(new GetSystemSettingsQuery(), ct);
    }

    [HttpPut]
    [Authorize(Policy = AuthPolicies.AdminOnly)]
    public async Task Update(UpdateSystemSettingsCommand command, CancellationToken ct)
    {
        await sender.Send(command, ct);
    }
}
