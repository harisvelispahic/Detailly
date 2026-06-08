namespace Detailly.Application.Modules.Auth.Commands.LinkExternalAccount;

public sealed class LinkExternalAccountCommandDto
{
    public required string AccessToken { get; init; }
    public required string RefreshToken { get; init; }
    public required DateTime AccessTokenExpiresAtUtc { get; init; }
    public required DateTime RefreshTokenExpiresAtUtc { get; init; }
}
