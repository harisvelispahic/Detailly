using Detailly.Application.Modules.Identity.User.Commands.ChangePassword;

public sealed class ChangePasswordCommandHandler(
    IAppDbContext context,
    IAppCurrentUser appCurrentUser,
    IPasswordHasher<ApplicationUserEntity> passwordHasher
) : IRequestHandler<ChangePasswordCommand, Unit>
{
    public async Task<Unit> Handle(ChangePasswordCommand request, CancellationToken ct)
    {
        if (!appCurrentUser.IsAuthenticated || appCurrentUser.ApplicationUserId is null)
            throw new DetaillyUnauthorizedException("User is not authenticated.");

        var currentUserId = appCurrentUser.ApplicationUserId.Value;

        var user = await context.ApplicationUsers
            .FirstOrDefaultAsync(x => x.Id == currentUserId && !x.IsDeleted, ct);

        if (user == null)
            throw new DetaillyNotFoundException("User not found.");

        // Verify old password
        var verifyResult = passwordHasher.VerifyHashedPassword(
            user,
            user.PasswordHash,
            request.CurrentPassword
        );

        if (verifyResult == PasswordVerificationResult.Failed)
            throw new ValidationException("Current password is incorrect.");

        // Hash and set new password
        user.PasswordHash = passwordHasher.HashPassword(user, request.NewPassword);

        // Revoke existing tokens
        user.TokenVersion++;

        user.ModifiedAtUtc = DateTime.UtcNow;

        await context.SaveChangesAsync(ct);

        return Unit.Value;
    }
}
