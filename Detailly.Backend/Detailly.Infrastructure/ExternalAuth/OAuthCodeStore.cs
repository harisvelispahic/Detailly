using Detailly.Application.Abstractions;
using Detailly.Application.Modules.Auth.Commands.ExternalLogin;
using Microsoft.Extensions.Caching.Memory;

namespace Detailly.Infrastructure.ExternalAuth;

public sealed class OAuthCodeStore(IMemoryCache cache) : IOAuthCodeStore
{
    public Task StoreAsync(string code, ExternalLoginCommandDto payload, TimeSpan ttl, CancellationToken ct = default)
    {
        cache.Set(CacheKey(code), payload, ttl);
        return Task.CompletedTask;
    }

    public Task<ExternalLoginCommandDto?> RedeemAsync(string code, CancellationToken ct = default)
    {
        if (!cache.TryGetValue<ExternalLoginCommandDto>(CacheKey(code), out var payload))
            return Task.FromResult<ExternalLoginCommandDto?>(null);

        cache.Remove(CacheKey(code));
        return Task.FromResult<ExternalLoginCommandDto?>(payload);
    }

    private static string CacheKey(string code) => $"oauth_code:{code}";
}
