using Detailly.Application.Modules.Auth.Commands.ExternalLogin;

namespace Detailly.Application.Abstractions;

public interface IExternalAuthCallbackBuilder
{
    Task<string> BuildAsync(string returnUrl, ExternalLoginCommandDto result, CancellationToken ct = default);
}
