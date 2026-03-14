using Detailly.Domain.Common.Enums;

namespace Detailly.Application.Modules.Booking.Locations.Commands.Update;

public sealed class UpdateLocationCommandHandler(IAppDbContext context, IAppCurrentUser appCurrentUser)
    : IRequestHandler<UpdateLocationCommand, Unit>
{
    public async Task<Unit> Handle(UpdateLocationCommand request, CancellationToken ct)
    {
        var now = DateTime.UtcNow;

        if (!appCurrentUser.IsAuthenticated)
            throw new DetaillyBusinessRuleException("AUTH_REQUIRED", "Authentication required.");

        if (!appCurrentUser.IsAdmin && !appCurrentUser.IsManager)
            throw new DetaillyBusinessRuleException("FORBIDDEN", "Only admin/manager can manage locations.");

        var location = await context.Locations
            .FirstOrDefaultAsync(l => l.Id == request.Id && !l.IsDeleted, ct);

        if (location is null)
            throw new DetaillyNotFoundException("Location not found.");

        if (!string.IsNullOrWhiteSpace(request.Name))
            location.Name = request.Name.Trim();

        if (request.Description is not null)
            location.Description = request.Description.Trim();

        if (request.AddressId is not null)
        {
            var exists = await context.Addresses
                .AnyAsync(a => a.Id == request.AddressId.Value && !a.IsDeleted, ct);

            if (!exists)
                throw new DetaillyNotFoundException("Address not found.");

            location.AddressId = request.AddressId.Value;
        }

        if (request.LocationType is not null)
            location.LocationType = request.LocationType.Value;

        // Bays rules
        if (location.LocationType == LocationType.Shop)
        {
            if (request.TotalBays is not null)
            {
                if (request.TotalBays.Value <= 0)
                    throw new DetaillyBusinessRuleException("LOCATION_BAYS_REQUIRED", "Shop locations must have TotalBays > 0.");

                location.TotalBays = request.TotalBays.Value;
            }
            else if (location.TotalBays <= 0)
            {
                throw new DetaillyBusinessRuleException("LOCATION_BAYS_REQUIRED", "Shop locations must have TotalBays > 0.");
            }
        }
        else
        {
            // address-type -> force 0
            location.TotalBays = 0;
        }

        location.ModifiedAtUtc = now;

        await context.SaveChangesAsync(ct);
        return Unit.Value;
    }
}