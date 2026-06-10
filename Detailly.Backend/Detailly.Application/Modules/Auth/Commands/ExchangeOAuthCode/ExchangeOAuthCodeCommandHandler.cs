using Detailly.Application.Modules.Auth.Commands.ExternalLogin;

namespace Detailly.Application.Modules.Auth.Commands.ExchangeOAuthCode;

public sealed class ExchangeOAuthCodeCommandHandler(IOAuthCodeStore codeStore)
    : IRequestHandler<ExchangeOAuthCodeCommand, ExternalLoginCommandDto>
{
    public async Task<ExternalLoginCommandDto> Handle(ExchangeOAuthCodeCommand request, CancellationToken ct)
    {
        var payload = await codeStore.RedeemAsync(request.Code, ct);
        if (payload is null)
            throw new DetaillyGoneException("OAuth code has expired or was already used.");
        return payload;
    }
}
