using System.Security.Claims;

namespace Detailly.Application.Modules.Auth.Commands.ExternalLogin;

public sealed record ExternalLoginCommand(
    string Provider,
    ClaimsPrincipal Principal
) : IRequest<ExternalLoginCommandDto>;
