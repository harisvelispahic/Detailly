using Microsoft.EntityFrameworkCore;

namespace Detailly.Application.Modules.Auth.Commands.CompleteOAuthSetup;

public sealed class CompleteOAuthSetupCommandHandler(
    IAppDbContext ctx,
    IAppCurrentUser currentUser)
    : IRequestHandler<CompleteOAuthSetupCommand, Unit>
{
    public async Task<Unit> Handle(CompleteOAuthSetupCommand request, CancellationToken ct)
    {
        if (!currentUser.IsAuthenticated || currentUser.ApplicationUserId is null)
            throw new DetaillyUnauthorizedException("User is not authenticated.");

        var user = await ctx.ApplicationUsers
            .Include(x => x.ExternalLogins)
            .FirstOrDefaultAsync(x => x.Id == currentUser.ApplicationUserId.Value && !x.IsDeleted, ct);

        if (user is null)
            throw new DetaillyNotFoundException("User not found.");

        if (!user.ExternalLogins.Any())
            throw new DetaillyForbiddenException("Profile setup is only available for OAuth accounts.");

        var normalizedUsername = request.Username.Trim();
        var usernameExists = await ctx.ApplicationUsers
            .AnyAsync(x => x.Username == normalizedUsername && x.Id != user.Id, ct);

        if (usernameExists)
            throw new DetaillyConflictException("Username is already taken.");

        user.Username = normalizedUsername;
        user.Phone = string.IsNullOrWhiteSpace(request.Phone) ? null : request.Phone.Trim();
        user.IsFleet = false;
        user.IsProfileComplete = true;
        user.ModifiedAtUtc = DateTime.UtcNow;

        await ctx.SaveChangesAsync(ct);

        return Unit.Value;
    }
}
