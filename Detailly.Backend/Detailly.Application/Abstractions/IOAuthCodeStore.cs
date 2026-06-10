using Detailly.Application.Modules.Auth.Commands.ExternalLogin;

namespace Detailly.Application.Abstractions;

public interface IOAuthCodeStore
{
    Task StoreAsync(string code, ExternalLoginCommandDto payload, TimeSpan ttl, CancellationToken ct = default);
    Task<ExternalLoginCommandDto?> RedeemAsync(string code, CancellationToken ct = default);
}
