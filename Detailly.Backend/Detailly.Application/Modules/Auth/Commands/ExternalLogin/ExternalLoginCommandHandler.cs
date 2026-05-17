using Detailly.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using MediatR;

namespace Detailly.Application.Modules.Auth.Commands.ExternalLogin
{
    public sealed class ExternalLoginCommandHandler(IAppDbContext ctx, IJwtTokenService jwt) 
        : IRequestHandler<ExternalLoginCommand, ExternalLoginCommandDto>
    {
        public async Task<ExternalLoginCommandDto> Handle(
            ExternalLoginCommand request,
            CancellationToken ct)
        {
            var email = request.Principal.FindFirstValue(ClaimTypes.Email);
            var providerUserId = request.Principal.FindFirstValue(ClaimTypes.NameIdentifier);

            if (email is null || providerUserId is null)
                throw new DetaillyUnauthorizedException("External login failed: missing claims.");

            var external = await ctx.UserExternalLogins
                .Include(x => x.ApplicationUser)
                .FirstOrDefaultAsync(x =>
                    x.Provider == request.Provider &&
                    x.ProviderUserId == providerUserId, ct);

            ApplicationUserEntity user;
            bool isSetupRequired;

            if (external is not null)
            {
                user = external.ApplicationUser!;
                isSetupRequired = !user.IsProfileComplete;
            }
            else
            {
                var existingUser = await ctx.ApplicationUsers
                    .FirstOrDefaultAsync(x => x.Email == email.Trim().ToLowerInvariant() && !x.IsDeleted, ct);

                if (existingUser is null)
                {
                    user = CreateNewUser(request.Principal);
                    ctx.ApplicationUsers.Add(user);
                    isSetupRequired = true;
                }
                else
                {
                    user = existingUser;
                    isSetupRequired = !user.IsProfileComplete;
                }

                ctx.UserExternalLogins.Add(new UserExternalLoginEntity
                {
                    Provider = request.Provider,
                    ProviderUserId = providerUserId,
                    ApplicationUser = user,
                    Email = email
                });
            }

            if (!user.IsEnabled || user.IsDeleted)
                throw new DetaillyUnauthorizedException("Account is disabled.");

            // Save first so new users get their DB-generated Id before it's stamped into JWT claims.
            await ctx.SaveChangesAsync(ct);

            var pair = jwt.IssueTokens(user);

            ctx.RefreshTokens.Add(new RefreshTokenEntity
            {
                TokenHash = pair.RefreshTokenHash,
                ExpiresAtUtc = pair.RefreshTokenExpiresAtUtc,
                ApplicationUserId = user.Id,
                Fingerprint = null
            });

            await ctx.SaveChangesAsync(ct);

            return new ExternalLoginCommandDto
            {
                AccessToken = pair.AccessToken,
                RefreshToken = pair.RefreshTokenRaw,
                AccessTokenExpiresAtUtc = pair.AccessTokenExpiresAtUtc,
                RefreshTokenExpiresAtUtc = pair.RefreshTokenExpiresAtUtc,
                IsSetupRequired = isSetupRequired
            };
        }

        private static ApplicationUserEntity CreateNewUser(ClaimsPrincipal principal)
        {
            var firstName = principal.FindFirstValue(ClaimTypes.GivenName)
                ?? principal.FindFirstValue(ClaimTypes.Name)?.Split(' ')[0]
                ?? "Unknown";
            var lastName = principal.FindFirstValue(ClaimTypes.Surname)
                ?? (principal.FindFirstValue(ClaimTypes.Name)?.Split(' ') is { Length: > 1 } parts ? parts[^1] : "");
            var email = principal.FindFirstValue(ClaimTypes.Email)!.Trim().ToLowerInvariant();

            // Generate a unique temp username — user will choose their real one during setup.
            var tempUsername = $"user_{Guid.NewGuid().ToString("N")[..8]}";

            return new ApplicationUserEntity
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                Username = tempUsername,
                PasswordHash = ApplicationUserEntity.ExternalOnlyPasswordHash,
                IsEnabled = true,
                IsAdmin = false,
                IsEmployee = false,
                IsManager = false,
                IsFleet = false,
                IsProfileComplete = false
            };
        }
    }
}
