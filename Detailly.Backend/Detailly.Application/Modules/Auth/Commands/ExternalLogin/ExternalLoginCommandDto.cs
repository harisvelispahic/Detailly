namespace Detailly.Application.Modules.Auth.Commands.ExternalLogin;

public sealed class ExternalLoginCommandDto
{
    public required bool RequiresLinking { get; init; }

    // Present only when RequiresLinking = true
    public string? PendingLinkToken { get; init; }

    // Present only when RequiresLinking = false
    public string? AccessToken { get; init; }
    public string? RefreshToken { get; init; }
    public DateTime? AccessTokenExpiresAtUtc { get; init; }
    public DateTime? RefreshTokenExpiresAtUtc { get; init; }
    public bool IsSetupRequired { get; init; }
}
