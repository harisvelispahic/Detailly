using Detailly.Application.Abstractions;
using Detailly.Application.Modules.Auth.Commands.CompleteOAuthSetup;
using Detailly.Application.Modules.Auth.Commands.ExternalLogin;
using Detailly.Application.Modules.Auth.Commands.LinkExternalAccount;
using Microsoft.AspNetCore.Authentication;

namespace Detailly.API.Controllers;

[ApiController]
[Route("auth/external")]
public sealed class ExternalAuthController(
    ISender sender,
    IExternalAuthService externalAuthService,
    IReturnUrlValidator returnUrlValidator,
    IExternalAuthCallbackBuilder callbackBuilder) : ControllerBase
{
    [HttpGet("google")]
    [AllowAnonymous]
    public IActionResult GoogleLogin([FromQuery] string returnUrl)
    {
        if (!returnUrlValidator.IsAllowed(returnUrl))
            return BadRequest("Invalid returnUrl.");

        var finalizeUrl = Url.Action(nameof(GoogleFinalize), "ExternalAuth", new { returnUrl }, Request.Scheme);
        return Challenge(new AuthenticationProperties { RedirectUri = finalizeUrl }, "Google");
    }

    [HttpGet("google/finalize")]
    [AllowAnonymous]
    public async Task<IActionResult> GoogleFinalize([FromQuery] string returnUrl, CancellationToken ct)
    {
        var principal = await externalAuthService.ExchangeExternalCookieAsync(ct);
        if (principal is null)
            return Unauthorized();

        var result = await sender.Send(new ExternalLoginCommand("Google", principal), ct);
        return Redirect(callbackBuilder.Build(returnUrl, result));
    }

    [HttpPost("link")]
    [AllowAnonymous]
    public async Task<ActionResult<LinkExternalAccountCommandDto>> LinkAccount(
        [FromBody] LinkExternalAccountCommand command,
        CancellationToken ct)
    {
        var result = await sender.Send(command, ct);
        return Ok(result);
    }

    [HttpPost("setup")]
    [Authorize]
    public async Task CompleteSetup(CompleteOAuthSetupCommand command, CancellationToken ct)
    {
        await sender.Send(command, ct);
    }
}
