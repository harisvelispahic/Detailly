namespace Detailly.Application.Modules.Identity.Staff.Commands.Update;

public sealed class UpdateStaffMemberCommandHandler(
    IAppDbContext context,
    IAppAuthorizationService authService,
    IAppCurrentUser appCurrentUser)
    : IRequestHandler<UpdateStaffMemberCommand, Unit>
{
    public async Task<Unit> Handle(UpdateStaffMemberCommand request, CancellationToken ct)
    {
        authService.EnsureAdminOrManager();

        var isAdmin = appCurrentUser.IsAdmin;

        var user = await context.ApplicationUsers
            .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

        if (user is null)
            throw new DetaillyNotFoundException("Staff member not found.");

        if (!(user.IsEmployee || user.IsManager))
            throw new DetaillyForbiddenException("Target user is not a staff member.");

        // Managers cannot update other managers
        if (!isAdmin && user.IsManager)
            throw new DetaillyForbiddenException("Managers cannot update other managers.");

        if (request.Email != null)
        {
            var normalizedEmail = request.Email.Trim().ToLower();
            var exists = await context.ApplicationUsers
                .AnyAsync(x => x.Email == normalizedEmail && x.Id != request.Id, ct);
            if (exists)
                throw new DetaillyConflictException("Email already exists.");
            user.Email = normalizedEmail;
        }

        if (request.Username != null)
        {
            var normalizedUsername = request.Username.Trim();
            var exists = await context.ApplicationUsers
                .AnyAsync(x => x.Username == normalizedUsername && x.Id != request.Id, ct);
            if (exists)
                throw new DetaillyConflictException("Username already exists.");
            user.Username = normalizedUsername;
        }

        if (request.FirstName != null) user.FirstName = request.FirstName.Trim();
        if (request.LastName != null) user.LastName = request.LastName.Trim();
        if (request.Phone is not null)
            user.Phone = string.IsNullOrWhiteSpace(request.Phone) ? null : request.Phone.Trim();

        if (request.IsManager.HasValue)
        {
            if (!isAdmin)
                throw new DetaillyForbiddenException("Only admins can change a staff member's role.");

            user.IsManager = request.IsManager.Value;
            user.IsEmployee = !request.IsManager.Value;
        }

        user.ModifiedAtUtc = DateTime.UtcNow;
        await context.SaveChangesAsync(ct);
        return Unit.Value;
    }
}
