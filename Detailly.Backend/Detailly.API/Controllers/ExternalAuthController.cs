using Detailly.Application.Modules.Auth.Commands.ExternalLogin;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/auth/external")]
public sealed class ExternalAuthController : ControllerBase
{
    [HttpGet("google")]
    [AllowAnonymous]
    public IActionResult GoogleLogin([FromQuery] string returnUrl)
    {
        // Force ABSOLUTE redirect URI (Google requires this)
        var callbackUrl = Url.Action(
            action: nameof(GoogleCallback),
            controller: "ExternalAuth",
            values: new { returnUrl },
            protocol: Request.Scheme);

        Console.WriteLine("=== GOOGLE REDIRECT URI ===");
        Console.WriteLine(callbackUrl);
        Console.WriteLine("==========================");

        var props = new AuthenticationProperties
        {
            RedirectUri = callbackUrl!
        };

        return Challenge(props, "Google");
    }


    [HttpGet("google/callback")]
    [AllowAnonymous]
    public async Task<IActionResult> GoogleCallback(
        [FromQuery] string returnUrl,
        [FromServices] IMediator mediator)
    {
        // Authenticate the EXTERNAL cookie, not "Google"
        var result = await HttpContext.AuthenticateAsync("External");

        if (!result.Succeeded || result.Principal is null)
            return Unauthorized();

        // Optional cleanup
        await HttpContext.SignOutAsync("External");

        var tokens = await mediator.Send(new ExternalLoginCommand(
            Provider: "Google",
            Principal: result.Principal));

        var redirectUrl =
            $"{returnUrl}?accessToken={Uri.EscapeDataString(tokens.AccessToken)}" +
            $"&refreshToken={Uri.EscapeDataString(tokens.RefreshToken)}";

        return Redirect(redirectUrl);
    }

    [HttpGet("/test")]
    [AllowAnonymous]
    public IActionResult Test([FromQuery] string accessToken, [FromQuery] string refreshToken)
    => Ok(new { accessToken = accessToken?.Length, refreshToken = refreshToken?.Length });
}
