using Detailly.Application.Abstractions;
using Detailly.Application.Modules.Auth.Commands.ExternalLogin;

namespace Detailly.Infrastructure.ExternalAuth;

public sealed class ExternalAuthCallbackBuilder : IExternalAuthCallbackBuilder
{
    public string Build(string returnUrl, ExternalLoginCommandDto result)
    {
        if (result.RequiresLinking)
        {
            return $"{returnUrl}#requiresLinking=true" +
                   $"&pendingLinkToken={Uri.EscapeDataString(result.PendingLinkToken!)}";
        }

        return $"{returnUrl}#accessToken={Uri.EscapeDataString(result.AccessToken!)}" +
               $"&refreshToken={Uri.EscapeDataString(result.RefreshToken!)}" +
               $"&isSetupRequired={result.IsSetupRequired.ToString().ToLowerInvariant()}";
    }
}
