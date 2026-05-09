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

        if (!isAdmin && user.IsManager)
            throw new DetaillyForbiddenException("Managers cannot delete other managers.");

        var now = DateTime.UtcNow;

        // Staff-specific: cascade shifts. BookingEmployeeAssignments are kept for historical records.
        var shifts = await context.EmployeeShifts
            .Where(s => s.EmployeeId == request.Id)
            .ToListAsync(ct);

        foreach (var shift in shifts)
        {
            shift.IsDeleted = true;
            shift.ModifiedAtUtc = now;
        }

        // User-level cascades applied idempotently (staff typically won't have these)
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
