using Detailly.Application.Modules.Identity.User.Commands.ChangePassword;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

public sealed class ChangePasswordCommandHandler(
    IAppDbContext context,
    IAppCurrentUser appCurrentUser,
    IPasswordHasher<ApplicationUserEntity> passwordHasher
) : IRequestHandler<ChangePasswordCommand, Unit>
{
    public async Task<Unit> Handle(ChangePasswordCommand request, CancellationToken ct)
    {
        var user = await context.ApplicationUsers
            .FirstOrDefaultAsync(x => x.Id == appCurrentUser.ApplicationUserId, ct);

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

        // Optional: revoke refresh tokens
        user.TokenVersion++;

        await context.SaveChangesAsync(ct);

        return Unit.Value;
    }
}
