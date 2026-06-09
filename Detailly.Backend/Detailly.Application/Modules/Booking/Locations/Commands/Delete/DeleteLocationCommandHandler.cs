namespace Detailly.Application.Modules.Booking.Locations.Commands.Delete;

public sealed class DeleteLocationCommandHandler(IAppDbContext context, IAppAuthorizationService authService)
    : IRequestHandler<DeleteLocationCommand, Unit>
{
    public async Task<Unit> Handle(DeleteLocationCommand request, CancellationToken ct)
    {
        var now = DateTime.UtcNow;

        authService.EnsureAdminOrManager();

        var location = await context.Locations
            .FirstOrDefaultAsync(l => l.Id == request.Id && !l.IsDeleted, ct);

        if (location is null)
            return Unit.Value; // idempotent

        var hasBookings = await context.Bookings
            .AnyAsync(b => b.ShopLocationId == request.Id, ct);

        if (hasBookings)
            throw new DetaillyBusinessRuleException(
                "LOCATION_HAS_BOOKINGS",
                "Cannot delete a location that has existing bookings.");

        var shifts = await context.EmployeeShifts
            .Where(s => s.ShopLocationId == request.Id && !s.IsDeleted)
            .ToListAsync(ct);

        if (shifts.Any(s => s.EndUtc > now))
            throw new DetaillyBusinessRuleException(
                "LOCATION_HAS_FUTURE_SHIFTS",
                "Cannot delete a location that has upcoming shifts. Remove or reassign them first.");

        foreach (var shift in shifts)
        {
            shift.IsDeleted = true;
            shift.ModifiedAtUtc = now;
        }

        var openingHours = await context.LocationOpeningHours
            .Where(h => h.ShopLocationId == request.Id)
            .ToListAsync(ct);

        foreach (var oh in openingHours)
        {
            oh.IsDeleted = true;
            oh.ModifiedAtUtc = now;
        }

        // Address is owned 1:1 by the location
        var address = await context.Addresses
            .FirstOrDefaultAsync(a => a.Id == location.AddressId, ct);

        if (address is not null)
        {
            address.IsDeleted = true;
            address.ModifiedAtUtc = now;
        }

        location.IsDeleted = true;
        location.ModifiedAtUtc = now;

        await context.SaveChangesAsync(ct);
        return Unit.Value;
    }
}
