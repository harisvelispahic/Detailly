namespace Detailly.Application.Modules.Auth.Commands.LinkExternalAccount;

public sealed record LinkExternalAccountCommand(
    string PendingLinkToken,
    string Password
) : IRequest<LinkExternalAccountCommandDto>;
