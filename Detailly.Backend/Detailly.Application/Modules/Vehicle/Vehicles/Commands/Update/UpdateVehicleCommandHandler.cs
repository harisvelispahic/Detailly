
namespace Detailly.Application.Modules.Vehicle.Vehicles.Commands.Update;

public class UpdateVehicleCommandHandler(IAppDbContext ctx)
    : IRequestHandler<UpdateVehicleCommand, Unit>
{
    public async Task<Unit> Handle(UpdateVehicleCommand request, CancellationToken ct)
    {
        // Find the vehicle
        var vehicle = await ctx.Vehicles
            .FirstOrDefaultAsync(x => x.Id == request.Id, ct);

        if (vehicle is null)
            throw new DetaillyNotFoundException($"Vehicle (ID={request.Id}) was not found.");

        if (vehicle.IsDeleted)
            throw new DetaillyConflictException("Cannot update a deleted vehicle.");


        var newLicencePlate = request.LicencePlate?.Trim().ToUpper();

        // --- Check for duplicate licence plate if updated ---
        if (newLicencePlate is not null && vehicle.LicencePlate.ToUpper() != newLicencePlate)
        {
            var licenceExists = await ctx.Vehicles
                .AnyAsync(x => x.LicencePlate.ToUpper() == newLicencePlate, ct);

            if (licenceExists)
                throw new DetaillyConflictException("A vehicle with this licence plate already exists.");
        }

        // Update only provided fields
        if (request.Brand != null)
            vehicle.Brand = request.Brand.Trim();

        if (request.Model != null)
            vehicle.Model = request.Model.Trim();

        if (request.YearOfManufacture.HasValue)
            vehicle.YearOfManufacture = request.YearOfManufacture.Value;

        if (newLicencePlate != null)
            vehicle.LicencePlate = newLicencePlate.Trim();

        if (request.Notes != null)
            vehicle.Notes = request.Notes.Trim();

        if (request.VehicleCategoryId.HasValue)
            vehicle.VehicleCategoryId = request.VehicleCategoryId.Value;

        await ctx.SaveChangesAsync(ct);

        return Unit.Value;
    }
}
