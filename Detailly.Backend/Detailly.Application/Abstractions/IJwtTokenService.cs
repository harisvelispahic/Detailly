namespace Detailly.Application.Abstractions;

public sealed class JwtTokenPair
{
    public string AccessToken { get; init; }
    public DateTime AccessTokenExpiresAtUtc { get; init; }

    public string RefreshTokenRaw { get; init; }
    public string RefreshTokenHash { get; init; }
    public DateTime RefreshTokenExpiresAtUtc { get; init; }
}

public interface IJwtTokenService
{
    /// <summary>Issues an access token and returns all its technical details.</summary>
    JwtTokenPair IssueTokens(ApplicationUserEntity user);

    /// <summary>Hashes the refresh token for storage in the database.</summary>
    string HashRefreshToken(string rawToken);

    /// <summary>Issues a short-lived token that proves a provider identity was verified, used to authorise an account-link step.</summary>
    string IssuePendingLinkToken(string provider, string providerUserId, string email);

    /// <summary>Validates a pending link token and extracts its claims. Returns false if expired, tampered, or wrong type.</summary>
    bool TryValidatePendingLinkToken(string token, out string provider, out string providerUserId, out string email);
}