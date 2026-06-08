using Detailly.Application.Abstractions;
using Detailly.Shared.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Detailly.Infrastructure.Common;

public sealed class JwtTokenService : IJwtTokenService
{
    private readonly JwtOptions _jwt;
    private readonly TimeProvider _time;

    public JwtTokenService(IOptions<JwtOptions> options, TimeProvider time)
    {
        _jwt = options?.Value ?? throw new ArgumentNullException(nameof(options));
        _time = time ?? throw new ArgumentNullException(nameof(time));
    }

    public JwtTokenPair IssueTokens(ApplicationUserEntity user)
    {
        // Now from TimeProvider (consistent with the rest of the app)
        var nowInstant = _time.GetUtcNow();
        var nowUtc = nowInstant.UtcDateTime;
        var accessExpires = nowInstant.AddMinutes(_jwt.AccessTokenMinutes).UtcDateTime;
        var refreshExpires = nowInstant.AddDays(_jwt.RefreshTokenDays).UtcDateTime;

        // --- Claims (including jti/aud for standard compliance) ---
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(ClaimTypes.NameIdentifier,   user.Id.ToString()),
            new(ClaimTypes.Email,            user.Email),
            new("is_admin",    user.IsAdmin.ToString().ToLowerInvariant()),
            new("is_manager",  user.IsManager.ToString().ToLowerInvariant()),
            new("is_employee", user.IsEmployee.ToString().ToLowerInvariant()),
            new("is_fleet", user.IsFleet.ToString().ToLowerInvariant()),
            new("ver",         user.TokenVersion.ToString()),
            new(JwtRegisteredClaimNames.Iat, ToUnixTimeSeconds(nowInstant).ToString(), ClaimValueTypes.Integer64),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N"))
        };

        // --- Signature ---
        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
        var creds = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        // --- access token (JWT) ---
        var jwt = new JwtSecurityToken(
            issuer: _jwt.Issuer,
            audience: _jwt.Audience,
            claims: claims,
            notBefore: nowUtc,
            expires: accessExpires,
            signingCredentials: creds
        );

        var accessToken = new JwtSecurityTokenHandler().WriteToken(jwt);

        // --- refresh token (raw + hash) ---
        var refreshRaw = GenerateRefreshTokenRaw(64); // base64url
        var refreshHash = HashRefreshToken(refreshRaw); // base64url hash

        return new JwtTokenPair
        {
            AccessToken = accessToken,
            AccessTokenExpiresAtUtc = accessExpires,
            RefreshTokenRaw = refreshRaw,
            RefreshTokenHash = refreshHash,
            RefreshTokenExpiresAtUtc = refreshExpires
        };
    }

    public string HashRefreshToken(string rawToken)
    {
        using var sha = SHA256.Create();
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(rawToken));
        // Use Base64Url to avoid problematic characters
        return Base64UrlEncoder.Encode(bytes);
    }

    private static string GenerateRefreshTokenRaw(int numBytes)
    {
        // Base64UrlEncoder from Microsoft.IdentityModel.Tokens (without + / =)
        var bytes = RandomNumberGenerator.GetBytes(numBytes);
        return Base64UrlEncoder.Encode(bytes);
    }

    public string IssuePendingLinkToken(string provider, string providerUserId, string email)
    {
        var nowInstant = _time.GetUtcNow();

        var claims = new List<Claim>
        {
            new("token_type", "pending_link"),
            new("provider",   provider),
            new(ClaimTypes.NameIdentifier, providerUserId),
            new(ClaimTypes.Email,          email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N"))
        };

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
        var creds      = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var jwt = new JwtSecurityToken(
            issuer:            _jwt.Issuer,
            audience:          _jwt.Audience,
            claims:            claims,
            notBefore:         nowInstant.UtcDateTime,
            expires:           nowInstant.AddMinutes(10).UtcDateTime,
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }

    public bool TryValidatePendingLinkToken(string token, out string provider, out string providerUserId, out string email)
    {
        provider = providerUserId = email = string.Empty;
        try
        {
            var handler = new JwtSecurityTokenHandler();
            var validationParams = new TokenValidationParameters
            {
                ValidateIssuer           = true,
                ValidIssuer              = _jwt.Issuer,
                ValidateAudience         = true,
                ValidAudience            = _jwt.Audience,
                ValidateLifetime         = true,
                IssuerSigningKey         = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key)),
                ClockSkew                = TimeSpan.Zero
            };

            var principal = handler.ValidateToken(token, validationParams, out _);

            if (principal.FindFirstValue("token_type") != "pending_link")
                return false;

            provider       = principal.FindFirstValue("provider")              ?? string.Empty;
            providerUserId = principal.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
            email          = principal.FindFirstValue(ClaimTypes.Email)          ?? string.Empty;

            return provider.Length > 0 && providerUserId.Length > 0 && email.Length > 0;
        }
        catch
        {
            return false;
        }
    }

    private static long ToUnixTimeSeconds(DateTimeOffset dto) => dto.ToUnixTimeSeconds();
}