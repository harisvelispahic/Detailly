namespace Detailly.Application.Modules.Shared.Address.Commands.Update;

public sealed class UpdateAddressCommandHandler(
    IAppDbContext context,
    IAppCurrentUser appCurrentUser)
    : IRequestHandler<UpdateAddressCommand, Unit>
{
    public async Task<Unit> Handle(UpdateAddressCommand request, CancellationToken ct)
    {
        if (!appCurrentUser.IsAuthenticated || appCurrentUser.ApplicationUserId is null)
            throw new DetaillyUnauthorizedException("User is not authenticated.");

        var userId = appCurrentUser.ApplicationUserId.Value;
        var isStaff = appCurrentUser.IsAdmin || appCurrentUser.IsManager;

        var address = await context.Addresses
            .FirstOrDefaultAsync(a =>
                a.Id == request.Id &&
                (isStaff || a.ApplicationUserId == userId),
                ct);

        if (address is null)
            throw new DetaillyNotFoundException($"Address with Id {request.Id} not found.");

        if (request.Street is not null)
            address.Street = request.Street.Trim();

        if (request.City is not null)
            address.City = request.City.Trim();

        if (request.PostalCode is not null)
            address.PostalCode = request.PostalCode.Trim();

        if (request.Region is not null)
            address.Region = string.IsNullOrWhiteSpace(request.Region) ? null : request.Region.Trim();

        if (request.Country is not null)
            address.Country = request.Country.Trim();

        if (request.Latitude.HasValue)
            address.Latitude = request.Latitude.Value;

        if (request.Longitude.HasValue)
            address.Longitude = request.Longitude.Value;

        await context.SaveChangesAsync(ct);
        return Unit.Value;
    }
}