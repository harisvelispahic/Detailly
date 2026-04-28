using Detailly.Domain.Entities.Payment;
using Detailly.Domain.Entities.Sales;

namespace Detailly.Application.Modules.Identity.Staff.Commands.Create;

public sealed class CreateStaffMemberCommandHandler(
    IAppDbContext context,
    IPasswordHasher<ApplicationUserEntity> passwordHasher,
    IAppCurrentUser appCurrentUser)
    : IRequestHandler<CreateStaffMemberCommand, int>
{
    public async Task<int> Handle(CreateStaffMemberCommand request, CancellationToken ct)
    {
        if (!appCurrentUser.IsAuthenticated)
            throw new DetaillyUnauthorizedException("User is not authenticated.");

        var isAdmin = appCurrentUser.IsAdmin;
        var isManager = appCurrentUser.IsManager;

        if (!isAdmin && !isManager)
            throw new DetaillyForbiddenException("Only admins and managers can create staff members.");

        // Managers can only create employees, not other managers
        if (!isAdmin && request.IsManager)
            throw new DetaillyForbiddenException("Managers can only create employees.");

        var email = request.Email.Trim().ToLower();
        var username = request.Username.Trim();

        var emailExists = await context.ApplicationUsers
            .AnyAsync(x => x.Email == email, ct);

        if (emailExists)
            throw new DetaillyConflictException("Email already exists.");

        var usernameExists = await context.ApplicationUsers
            .AnyAsync(x => x.Username == username, ct);

        if (usernameExists)
            throw new DetaillyConflictException("Username already exists.");

        var hash = passwordHasher.HashPassword(null!, request.Password);

        var user = new ApplicationUserEntity
        {
            FirstName = request.FirstName.Trim(),
            LastName = request.LastName.Trim(),
            Username = username,
            Email = email,
            Phone = request.Phone?.Trim(),

            IsAdmin = false,
            IsManager = request.IsManager,
            IsEmployee = !request.IsManager,
            IsFleet = false,
            IsEnabled = true,
            TokenVersion = 0,

            PasswordHash = hash
        };

        user.Wallet = new WalletEntity { ApplicationUser = user };
        user.Cart = new CartEntity { ApplicationUser = user };

        context.ApplicationUsers.Add(user);
        await context.SaveChangesAsync(ct);

        return user.Id;
    }
}
