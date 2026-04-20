using System.Security.Claims;

namespace Detailly.Application.Abstractions;

public interface IExternalAuthService
{
    Task<ClaimsPrincipal?> ExchangeExternalCookieAsync(CancellationToken ct);
}
