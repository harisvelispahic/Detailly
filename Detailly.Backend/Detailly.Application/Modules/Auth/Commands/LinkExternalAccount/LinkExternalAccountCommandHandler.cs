using System.Security.Claims;

namespace Detailly.Application.Modules.Auth.Commands.LinkExternalAccount
{
    public sealed class LinkExternalAccountCommandHandler(
        IAppDbContext ctx,
        IJwtTokenService jwt,
        IPasswordHasher<ApplicationUserEntity> hasher)
        : IRequestHandler<LinkExternalAccountCommand, LinkExternalAccountCommandDto>
    {
        public async Task<LinkExternalAccountCommandDto> Handle(
            LinkExternalAccountCommand request,
            CancellationToken ct)
        {
            if (!jwt.TryValidatePendingLinkToken(
                    request.PendingLinkToken,
                    out var provider,
                    out var providerUserId,
                    out var email))
            {
                throw new DetaillyUnauthorizedException("Invalid or expired link token.");
            }

            var user = await ctx.ApplicationUsers
                .FirstOrDefaultAsync(x => x.Email == email && !x.IsDeleted, ct)
                ?? throw new DetaillyNotFoundException("Account not found.");

            if (!user.IsEnabled)
                throw new DetaillyUnauthorizedException("Account is disabled.");

            if (user.PasswordHash == ApplicationUserEntity.ExternalOnlyPasswordHash)
                throw new DetaillyUnauthorizedException("This account uses OAuth login only.");

            var verify = hasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
            if (verify == PasswordVerificationResult.Failed)
                throw new DetaillyUnauthorizedException("Wrong credentials.");

            // Guard against a race where the link was already created (e.g. double-submit).
            var alreadyLinked = await ctx.UserExternalLogins.AnyAsync(
                x => x.Provider == provider && x.ProviderUserId == providerUserId, ct);

            if (!alreadyLinked)
            {
                ctx.UserExternalLogins.Add(new UserExternalLoginEntity
                {
                    Provider        = provider,
                    ProviderUserId  = providerUserId,
                    ApplicationUser = user,
                    Email           = email
                });
            }

            var pair = jwt.IssueTokens(user);

            ctx.RefreshTokens.Add(new RefreshTokenEntity
            {
                TokenHash         = pair.RefreshTokenHash,
                ExpiresAtUtc      = pair.RefreshTokenExpiresAtUtc,
                ApplicationUserId = user.Id,
                Fingerprint       = null
            });

            await ctx.SaveChangesAsync(ct);

            return new LinkExternalAccountCommandDto
            {
                AccessToken              = pair.AccessToken,
                RefreshToken             = pair.RefreshTokenRaw,
                AccessTokenExpiresAtUtc  = pair.AccessTokenExpiresAtUtc,
                RefreshTokenExpiresAtUtc = pair.RefreshTokenExpiresAtUtc
            };
        }
    }
}
