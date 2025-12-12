
namespace Detailly.Application.Modules.Vehicle.Vehicles.Commands.Delete;

public class DeleteVehicleCommandHandler(IAppDbContext context)
    : IRequestHandler<DeleteVehicleCommand, Unit>
{
    public async Task<Unit> Handle(DeleteVehicleCommand request, CancellationToken ct)
    {

        var vehicle = await context.Vehicles
            .FirstOrDefaultAsync(x => x.Id == request.Id, ct);

        if (vehicle is null)
            throw new DetaillyNotFoundException("Vehicle was not found.");

        vehicle.IsDeleted = true; // Soft delete
        await context.SaveChangesAsync(ct);

        return Unit.Value;
    }
}
