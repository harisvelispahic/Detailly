using Detailly.Application.Modules.Auth.Commands.ExternalLogin;

namespace Detailly.Application.Modules.Auth.Commands.ExchangeOAuthCode;

public sealed record ExchangeOAuthCodeCommand(string Code) : IRequest<ExternalLoginCommandDto>;
