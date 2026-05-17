using Detailly.Application.Abstractions;
using Detailly.Application.Modules.Auth.Commands.CompleteOAuthSetup;
using Detailly.Application.Modules.Auth.Commands.ExternalLogin;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;

namespace Detailly.API.Controllers;

[ApiController]
[Route("auth/external")]
public sealed class ExternalAuthController(
    ISender sender,
    IExternalAuthService externalAuthService,
    IConfiguration configuration) : ControllerBase
{
    [HttpGet("google")]
    [AllowAnonymous]
    public IActionResult GoogleLogin([FromQuery] string returnUrl)
    {
        if (!IsReturnUrlAllowed(returnUrl))
            return BadRequest("Invalid returnUrl.");

        var finalizeUrl = Url.Action(
            action: nameof(GoogleFinalize),
            controller: "ExternalAuth",
            values: new { returnUrl },
            protocol: Request.Scheme);

        return Challenge(new AuthenticationProperties { RedirectUri = finalizeUrl! }, "Google");
    }

    [HttpGet("google/finalize")]
    [AllowAnonymous]
    public async Task<IActionResult> GoogleFinalize([FromQuery] string returnUrl, CancellationToken ct)
    {
        var principal = await externalAuthService.ExchangeExternalCookieAsync(ct);
        if (principal is null)
            return Unauthorized();

        var tokens = await sender.Send(new ExternalLoginCommand("Google", principal), ct);

        return Redirect(
            $"{returnUrl}#accessToken={Uri.EscapeDataString(tokens.AccessToken)}" +
            $"&refreshToken={Uri.EscapeDataString(tokens.RefreshToken)}" +
            $"&isSetupRequired={tokens.IsSetupRequired.ToString().ToLowerInvariant()}");
    }

    [HttpPost("setup")]
    [Authorize]
    public async Task CompleteSetup(CompleteOAuthSetupCommand command, CancellationToken ct)
    {
        await sender.Send(command, ct);
    }

    private bool IsReturnUrlAllowed(string returnUrl)
    {
        if (!Uri.TryCreate(returnUrl, UriKind.Absolute, out var uri))
            return false;

        var allowed = configuration
            .GetSection("Authentication:AllowedReturnHosts")
            .Get<string[]>() ?? [];

        return allowed.Any(h => uri.Authority.Equals(h, StringComparison.OrdinalIgnoreCase));
    }
}
