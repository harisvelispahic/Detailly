
using Detailly.Domain.Entities.Vehicle;

namespace Detailly.Application.Modules.Vehicle.Vehicles.Commands.Create;

public class CreateVehicleCommandHandler(IAppDbContext context, IAppCurrentUser appCurrentUser)
    : IRequestHandler<CreateVehicleCommand, int>
{
    public async Task<int> Handle(CreateVehicleCommand request, CancellationToken cancellationToken)
    {

        // duplicates are checked by licence plate
        bool exists = await context.Vehicles
            .AnyAsync(x =>
                x.LicencePlate.ToUpper() == request.LicencePlate.Trim().ToUpper() &&
                x.ApplicationUserId == appCurrentUser.ApplicationUserId,
                cancellationToken);

        if (exists)
        {
            throw new DetaillyConflictException("Vehicle already exists.");
        }

        var vehicle = new VehicleEntity
        {
            Brand = request.Brand.Trim()!,
            Model = request.Model.Trim()!,
            YearOfManufacture = request.YearOfManufacture,
            ApplicationUserId = appCurrentUser.ApplicationUserId!.Value,
            VehicleCategoryId = request.VehicleCategoryId,
            LicencePlate = request.LicencePlate.Trim(),
            Notes = request.Notes?.Trim()
        };

        context.Vehicles.Add(vehicle);
        await context.SaveChangesAsync(cancellationToken);

        return vehicle.Id;
    }
}
