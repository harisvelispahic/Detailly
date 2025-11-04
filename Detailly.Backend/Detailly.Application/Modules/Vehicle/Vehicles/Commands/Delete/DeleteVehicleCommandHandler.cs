
namespace Detailly.Application.Modules.Vehicle.Vehicles.Commands.Delete;

public class DeleteVehicleCommandHandler(IAppDbContext context, IAppCurrentUser appCurrentUser)
    : IRequestHandler<DeleteVehicleCommand, Unit>
{
    public async Task<Unit> Handle(DeleteVehicleCommand request, CancellationToken cancellationToken)
    {
        if (appCurrentUser.UserId is null)
            throw new DetaillyBusinessRuleException("123", "User was not authenticated.");

        var vehicle = await context.Vehicles
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (vehicle is null)
            throw new DetaillyNotFoundException("Vehicle was not found.");

        vehicle.IsDeleted = true; // Soft delete
        await context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
