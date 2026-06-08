using Detailly.Application.Modules.Auth.Commands.ExternalLogin;

namespace Detailly.Application.Abstractions;

public interface IExternalAuthCallbackBuilder
{
    string Build(string returnUrl, ExternalLoginCommandDto result);
}
