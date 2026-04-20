using Detailly.Application.Abstractions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Detailly.Infrastructure.ExternalAuth;

public sealed class ExternalAuthService(IHttpContextAccessor httpContextAccessor) : IExternalAuthService
{
    public async Task<ClaimsPrincipal?> ExchangeExternalCookieAsync(CancellationToken ct)
    {
        var httpContext = httpContextAccessor.HttpContext!;

        var result = await httpContext.AuthenticateAsync("External");
        if (!result.Succeeded || result.Principal is null)
            return null;

        await httpContext.SignOutAsync("External");
        return result.Principal;
    }
}
