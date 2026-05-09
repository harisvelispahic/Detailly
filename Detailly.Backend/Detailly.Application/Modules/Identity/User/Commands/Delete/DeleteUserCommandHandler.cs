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

        if (!isAdmin && request.Id != currentUserId)
            throw new DetaillyForbiddenException("You are not allowed to delete this user.");

        var user = await context.ApplicationUsers
            .FirstOrDefaultAsync(x => x.Id == request.Id, ct);

        if (user is null || user.IsDeleted)
            throw new DetaillyNotFoundException("User was not found.");

        var hasBookings = await context.Bookings
            .AnyAsync(b => b.CustomerId == request.Id, ct);

        if (hasBookings)
            throw new DetaillyBusinessRuleException(
                "USER_HAS_BOOKINGS",
                "Cannot delete a user that has existing bookings.");

        var hasOrders = await context.Orders
            .AnyAsync(o => o.ApplicationUserId == request.Id, ct);

        if (hasOrders)
            throw new DetaillyBusinessRuleException(
                "USER_HAS_ORDERS",
                "Cannot delete a user that has existing orders.");

        var now = DateTime.UtcNow;

        var vehicles = await context.Vehicles
            .Where(v => v.ApplicationUserId == request.Id)
            .ToListAsync(ct);

        foreach (var v in vehicles)
        {
            v.IsDeleted = true;
            v.ModifiedAtUtc = now;
        }

        var addresses = await context.Addresses
            .Where(a => a.ApplicationUserId == request.Id)
            .ToListAsync(ct);

        foreach (var a in addresses)
        {
            a.IsDeleted = true;
            a.ModifiedAtUtc = now;
        }

        var wallet = await context.Wallet
            .FirstOrDefaultAsync(w => w.ApplicationUserId == request.Id, ct);

        if (wallet is not null)
        {
            wallet.IsDeleted = true;
            wallet.ModifiedAtUtc = now;
        }

        var cart = await context.Carts
            .FirstOrDefaultAsync(c => c.ApplicationUserId == request.Id, ct);

        if (cart is not null)
        {
            var cartItems = await context.CartItems
                .Where(ci => ci.CartId == cart.Id)
                .ToListAsync(ct);

            foreach (var ci in cartItems)
            {
                ci.IsDeleted = true;
                ci.ModifiedAtUtc = now;
            }

            cart.IsDeleted = true;
            cart.ModifiedAtUtc = now;
        }

        var savedProducts = await context.SavedProducts
            .Where(sp => sp.ApplicationUserId == request.Id)
            .ToListAsync(ct);

        foreach (var sp in savedProducts)
        {
            sp.IsDeleted = true;
            sp.ModifiedAtUtc = now;
        }

        var externalLogins = await context.UserExternalLogins
            .Where(l => l.ApplicationUserId == request.Id)
            .ToListAsync(ct);

        foreach (var login in externalLogins)
        {
            login.IsDeleted = true;
            login.ModifiedAtUtc = now;
        }

        user.IsDeleted = true;
        user.ModifiedAtUtc = now;
        user.TokenVersion++;

        await context.SaveChangesAsync(ct);
        return Unit.Value;
    }
}
