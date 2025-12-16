namespace Detailly.Application.Modules.Auth.Commands.ExternalLogin;

public sealed class ExternalLoginCommandDto
{
    public required string AccessToken { get; init; }
    public required string RefreshToken { get; init; }
    public required DateTime AccessTokenExpiresAtUtc { get; init; }
    public required DateTime RefreshTokenExpiresAtUtc { get; init; }
}
