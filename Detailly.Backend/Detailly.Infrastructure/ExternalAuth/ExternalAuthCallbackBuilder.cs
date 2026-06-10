using Detailly.Application.Abstractions;
using Detailly.Application.Modules.Auth.Commands.ExternalLogin;

namespace Detailly.Infrastructure.ExternalAuth;

public sealed class ExternalAuthCallbackBuilder(IOAuthCodeStore codeStore) : IExternalAuthCallbackBuilder
{
    private static readonly TimeSpan CodeTtl = TimeSpan.FromSeconds(60);

    public async Task<string> BuildAsync(string returnUrl, ExternalLoginCommandDto result, CancellationToken ct = default)
    {
        var code = Guid.NewGuid().ToString("N");
        await codeStore.StoreAsync(code, result, CodeTtl, ct);
        return $"{returnUrl}?code={Uri.EscapeDataString(code)}";
    }
}
