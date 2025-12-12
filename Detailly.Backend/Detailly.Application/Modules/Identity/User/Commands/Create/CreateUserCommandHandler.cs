
using Detailly.Domain.Entities.Payment;
using Detailly.Domain.Entities.Sales;

namespace Detailly.Application.Modules.Identity.User.Commands.Create;

public sealed class CreateUserCommandHandler
    (IAppDbContext context, IPasswordHasher<ApplicationUserEntity> passwordHasher)
    : IRequestHandler<CreateUserCommand, int>
{
    public async Task<int> Handle(CreateUserCommand request, CancellationToken ct)
    {
        // Normalize values
        var email = request.Email.Trim().ToLower();
        var username = request.Username.Trim();

        // Business rule: Email must be unique
        var emailExists = await context.ApplicationUsers
            .AnyAsync(x => x.Email == email, ct);

        if (emailExists)
            throw new DetaillyConflictException("Email already exists.");

        // Business rule: Username must be unique
        var usernameExists = await context.ApplicationUsers
            .AnyAsync(x => x.Username == username, ct);

        if (usernameExists)
            throw new DetaillyConflictException("Username already exists.");

        var hash = passwordHasher.HashPassword(null!, request.Password);

        // Create entity
        var user = new ApplicationUserEntity
        {
            FirstName = request.FirstName.Trim(),
            LastName = request.LastName.Trim(),
            Username = username,
            Email = email,
            Phone = request.Phone?.Trim(),
            CompanyName = request.CompanyName?.Trim(),

            IsFleet = request.IsFleet,

            // Defaults
            IsAdmin = false,
            IsManager = false,
            IsEmployee = false,
            IsEnabled = true,
            TokenVersion = 0,

            PasswordHash = hash
        };

        // Attach wallet using navigation instead of FK
        user.Wallet = new WalletEntity
        {
            // IMPORTANT: don't set ApplicationUserId manually
            ApplicationUser = user
        };

        user.Cart = new CartEntity
        {
            ApplicationUser = user
        };

        context.ApplicationUsers.Add(user);
        await context.SaveChangesAsync(ct);

        return user.Id;
    }
}
