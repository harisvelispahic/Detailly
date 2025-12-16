using System.Security.Claims;
using MediatR;

namespace Detailly.Application.Modules.Auth.Commands.ExternalLogin;

public sealed record ExternalLoginCommand(
    string Provider,
    ClaimsPrincipal Principal
) : IRequest<ExternalLoginCommandDto>;
