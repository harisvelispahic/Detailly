namespace Detailly.Application.Modules.Shared.Address.Commands.Delete;

public sealed class DeleteAddressCommandHandler(
    IAppDbContext context,
    IAppAuthorizationService authService)
    : IRequestHandler<DeleteAddressCommand, Unit>
{
    public async Task<Unit> Handle(DeleteAddressCommand request, CancellationToken ct)
    {
        var address = await context.Addresses
            .FirstOrDefaultAsync(a => a.Id == request.Id, ct);

        if (address is null)
            throw new DetaillyNotFoundException($"Address with Id {request.Id} was not found.");

        if (address.ApplicationUserId is { } ownerId)
            authService.EnsureOwnerOrStaff(ownerId, "address");
        else
            authService.EnsureAdminOrManager();

        var isUsedByLocation = await context.Locations
            .AnyAsync(x => x.AddressId == request.Id, ct);

        if (isUsedByLocation)
            throw new DetaillyBusinessRuleException(
                "ADDRESS_IN_USE",
                "Address cannot be deleted because it is used by a location.");

        var isUsedByOrder = await context.Orders
            .AnyAsync(x => x.ShipToAddressId == request.Id, ct);

        if (isUsedByOrder)
            throw new DetaillyBusinessRuleException(
                "ADDRESS_IN_USE",
                "Address cannot be deleted because it is used by an order.");

        var isUsedByBooking = await context.Bookings
            .AnyAsync(x => x.ServiceAddressId == request.Id, ct);

        if (isUsedByBooking)
            throw new DetaillyBusinessRuleException(
                "ADDRESS_IN_USE",
                "Address cannot be deleted because it is used by a booking.");

        context.Addresses.Remove(address);
        await context.SaveChangesAsync(ct);

        return Unit.Value;
    }
}