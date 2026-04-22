using Detailly.Domain.Entities.Shared;

namespace Detailly.Application.Modules.Identity.User.Commands.Update;

public sealed class UpdateUserCommandHandler(
    IAppDbContext context,
    IAppCurrentUser appCurrentUser)
    : IRequestHandler<UpdateUserCommand, Unit>
{
    public async Task<Unit> Handle(UpdateUserCommand request, CancellationToken ct)
    {
        if (!appCurrentUser.IsAuthenticated || appCurrentUser.ApplicationUserId is null)
            throw new DetaillyUnauthorizedException("User is not authenticated.");

        var isStaff = appCurrentUser.IsAdmin || appCurrentUser.IsManager;
        var currentUserId = appCurrentUser.ApplicationUserId.Value;

        // Only staff can update arbitrary users; normal users can only update themselves
        if (!isStaff && request.Id != currentUserId)
            throw new DetaillyForbiddenException("You are not allowed to update this user.");

        var user = await context.ApplicationUsers
            .Include(x => x.Image)
            .Include(x => x.ExternalLogins)
            .FirstOrDefaultAsync(x => x.Id == request.Id, ct);

        if (user == null)
            throw new DetaillyNotFoundException("User not found.");

        if (request.Email != null && user.ExternalLogins.Any())
            throw new DetaillyForbiddenException("Email cannot be changed for OAuth-registered accounts.");

        // PERSONAL INFO
        if (request.FirstName != null)
            user.FirstName = request.FirstName.Trim();

        if (request.LastName != null)
            user.LastName = request.LastName.Trim();

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

        if (request.Phone is not null)
            user.Phone = string.IsNullOrWhiteSpace(request.Phone) ? null : request.Phone.Trim();

        if (request.CompanyName != null)
            user.CompanyName = request.CompanyName.Trim();

        // IMAGE (unchanged)
        if (request.Image?.ImageUrl != null)
        {
            if (user.Image == null)
            {
                user.Image = new ImageEntity
                {
                    ImageUrl = request.Image.ImageUrl.Trim()
                };
            }
            else
            {
                user.Image.ImageUrl = request.Image.ImageUrl.Trim();
            }
        }

        user.ModifiedAtUtc = DateTime.UtcNow;

        await context.SaveChangesAsync(ct);
        return Unit.Value;
    }
}