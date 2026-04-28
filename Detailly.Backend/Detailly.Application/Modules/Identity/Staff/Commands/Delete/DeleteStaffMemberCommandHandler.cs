namespace Detailly.Application.Modules.Identity.Staff.Commands.Delete;

public sealed class DeleteStaffMemberCommandHandler(
    IAppDbContext context,
    IAppCurrentUser appCurrentUser)
    : IRequestHandler<DeleteStaffMemberCommand, Unit>
{
    public async Task<Unit> Handle(DeleteStaffMemberCommand request, CancellationToken ct)
    {
        if (!appCurrentUser.IsAuthenticated)
            throw new DetaillyUnauthorizedException("User is not authenticated.");

        var isAdmin = appCurrentUser.IsAdmin;
        var isManager = appCurrentUser.IsManager;

        if (!isAdmin && !isManager)
            throw new DetaillyForbiddenException("Only admins and managers can delete staff members.");

        var user = await context.ApplicationUsers
            .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

        if (user is null)
            throw new DetaillyNotFoundException("Staff member not found.");

        if (!(user.IsEmployee || user.IsManager))
            throw new DetaillyForbiddenException("Target user is not a staff member.");

        // Managers cannot delete other managers
        if (!isAdmin && user.IsManager)
            throw new DetaillyForbiddenException("Managers cannot delete other managers.");

        user.IsDeleted = true;
        user.ModifiedAtUtc = DateTime.UtcNow;
        user.TokenVersion++;

        await context.SaveChangesAsync(ct);
        return Unit.Value;
    }
}
