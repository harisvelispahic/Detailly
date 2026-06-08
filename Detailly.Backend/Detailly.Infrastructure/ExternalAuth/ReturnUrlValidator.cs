using Detailly.Application.Abstractions;
using Microsoft.Extensions.Configuration;

namespace Detailly.Infrastructure.ExternalAuth;

public sealed class ReturnUrlValidator(IConfiguration configuration) : IReturnUrlValidator
{
    public bool IsAllowed(string returnUrl)
    {
        if (!Uri.TryCreate(returnUrl, UriKind.Absolute, out var uri))
            return false;

        var allowed = configuration
            .GetSection("Authentication:AllowedReturnHosts")
            .Get<string[]>() ?? [];

        return allowed.Any(h => uri.Authority.Equals(h, StringComparison.OrdinalIgnoreCase));
    }
}
