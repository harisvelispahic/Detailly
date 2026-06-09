using Detailly.Domain.Entities.Vehicle;

namespace Detailly.Application.Modules.Vehicle.Vehicles.Commands.Create;

public class CreateVehicleCommandHandler(IAppDbContext context, IAppAuthorizationService authService)
    : IRequestHandler<CreateVehicleCommand, int>
{
    public async Task<int> Handle(CreateVehicleCommand request, CancellationToken ct)
    {
        var userId = authService.RequireUserId();

        bool exists = await context.Vehicles
            .AnyAsync(x =>
                x.LicencePlate.ToUpper() == request.LicencePlate.Trim().ToUpper() &&
                x.ApplicationUserId == userId,
                ct);

        if (exists)
            throw new DetaillyConflictException("Vehicle already exists.");

        var vehicle = new VehicleEntity
        {
            Brand = request.Brand.Trim()!,
            Model = request.Model.Trim()!,
            YearOfManufacture = request.YearOfManufacture,
            ApplicationUserId = userId,
            VehicleCategoryId = request.VehicleCategoryId,
            LicencePlate = request.LicencePlate.Trim(),
            Notes = request.Notes?.Trim()
        };

        context.Vehicles.Add(vehicle);
        await context.SaveChangesAsync(ct);

        return vehicle.Id;
    }
}
