namespace Detailly.Application.Modules.Auth.Commands.CompleteOAuthSetup;

public sealed class CompleteOAuthSetupCommand : IRequest<Unit>
{
    public required string Username { get; set; }
    public string? Phone { get; set; }
}
