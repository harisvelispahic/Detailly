namespace Detailly.Application.Modules.Vehicle.Vehicles.Commands.Delete;

public class DeleteVehicleCommandHandler(IAppDbContext context, IAppCurrentUser appCurrentUser)
    : IRequestHandler<DeleteVehicleCommand, Unit>
{
    public async Task<Unit> Handle(DeleteVehicleCommand request, CancellationToken ct)
    {
        if (!appCurrentUser.IsAuthenticated || appCurrentUser.ApplicationUserId is null)
            throw new DetaillyUnauthorizedException("User is not authenticated.");

        var vehicle = await context.Vehicles
            .FirstOrDefaultAsync(x => x.Id == request.Id, ct);

        if (vehicle is null)
            throw new DetaillyNotFoundException("Vehicle was not found.");

        // Only owner or admin can delete
        if (!appCurrentUser.IsAdmin && vehicle.ApplicationUserId != appCurrentUser.ApplicationUserId)
            throw new DetaillyForbiddenException("You are not allowed to delete this vehicle.");

        vehicle.IsDeleted = true; // Soft delete
        await context.SaveChangesAsync(ct);

        return Unit.Value;
    }
}
