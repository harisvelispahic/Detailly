namespace Detailly.Application.Modules.Identity.User.Commands.Delete;
public class DeleteUserCommandHandler(IAppDbContext context, IAppCurrentUser appCurrentUser)
        : IRequestHandler<DeleteUserCommand, Unit>
{
    public async Task<Unit> Handle(DeleteUserCommand request, CancellationToken ct)
    {
        if (!appCurrentUser.IsAuthenticated || appCurrentUser.ApplicationUserId is null)
            throw new DetaillyUnauthorizedException("User is not authenticated.");

        var currentUserId = appCurrentUser.ApplicationUserId.Value;
        var isAdmin = appCurrentUser.IsAdmin;

        // Only admins or the user themself can delete a user
        if (!isAdmin && request.Id != currentUserId)
            throw new DetaillyForbiddenException("You are not allowed to delete this user.");

        var user = await context.ApplicationUsers
            .FirstOrDefaultAsync(x => x.Id == request.Id, ct);

        if (user is null || user.IsDeleted)
            throw new DetaillyNotFoundException("User was not found.");

        // Soft delete
        user.IsDeleted = true;
        user.ModifiedAtUtc = DateTime.UtcNow;

        // Revoke tokens for safety
        user.TokenVersion++;

        await context.SaveChangesAsync(ct);

        return Unit.Value;
    }
}