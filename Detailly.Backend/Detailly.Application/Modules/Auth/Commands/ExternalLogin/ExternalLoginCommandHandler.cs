using Detailly.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using MediatR;

namespace Detailly.Application.Modules.Auth.Commands.ExternalLogin
{
    public sealed class ExternalLoginCommandHandler(
        IAppDbContext ctx,
        IJwtTokenService jwt
    ) : IRequestHandler<ExternalLoginCommand, ExternalLoginCommandDto>
    {
        public async Task<ExternalLoginCommandDto> Handle(
            ExternalLoginCommand request,
            CancellationToken ct)
        {
            var email = request.Principal.FindFirstValue(ClaimTypes.Email);
            var providerUserId = request.Principal.FindFirstValue(ClaimTypes.NameIdentifier);

            if (email is null || providerUserId is null)
                throw new DetaillyConflictException("External login failed.");

            // 1️⃣ Check external login table
            var external = await ctx.UserExternalLogins
                .Include(x => x.ApplicationUser)
                .FirstOrDefaultAsync(x =>
                    x.Provider == request.Provider &&
                    x.ProviderUserId == providerUserId, ct);

            ApplicationUserEntity user;

            if (external is not null)
            {
                // 2️⃣ Returning external user
                user = external.ApplicationUser!;
            }
            else
            {
                // 3️⃣ First-time external login
                user = await ctx.ApplicationUsers
                    .FirstOrDefaultAsync(x => x.Email == email, ct)
                    ?? CreateNewUser(request.Principal);

                ctx.ApplicationUsers.Add(user);

                ctx.UserExternalLogins.Add(new UserExternalLoginEntity
                {
                    Provider = request.Provider,
                    ProviderUserId = providerUserId,
                    ApplicationUser = user,
                    Email = email
                });
            }

            // 4️⃣ Issue tokens
            var pair = jwt.IssueTokens(user);

            await ctx.SaveChangesAsync(ct);

            return new ExternalLoginCommandDto
            {
                AccessToken = pair.AccessToken,
                RefreshToken = pair.RefreshTokenRaw,
                AccessTokenExpiresAtUtc = pair.AccessTokenExpiresAtUtc,
                RefreshTokenExpiresAtUtc = pair.RefreshTokenExpiresAtUtc
            };
        }

        private ApplicationUserEntity CreateNewUser(ClaimsPrincipal principal)
        {
            var name = principal.FindFirstValue(ClaimTypes.Name)?.Split(' ') ?? new[] { "Unknown", "User" };
            return new ApplicationUserEntity
            {
                FirstName = name[0],
                LastName = name.Length > 1 ? name[1] : "",
                Email = principal.FindFirstValue(ClaimTypes.Email)!,
                Username = principal.FindFirstValue(ClaimTypes.Email)!.Split('@')[0],
                PasswordHash = "", // external login, no password
                IsEnabled = true,
                IsAdmin = false,
                IsEmployee = false,
                IsManager = false,
                IsFleet = false
            };
        }
    }
}
