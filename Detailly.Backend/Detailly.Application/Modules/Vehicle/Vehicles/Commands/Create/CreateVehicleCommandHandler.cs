using Detailly.Application.Modules.Catalog.ProductCategories.Commands.Create;
using Detailly.Domain.Entities.Vehicle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Detailly.Application.Modules.Vehicle.Vehicles.Commands.Create;
public class CreateVehicleCommandHandler(IAppDbContext context)
    : IRequestHandler<CreateVehicleCommand, int>
{
    public async Task<int> Handle(CreateVehicleCommand request, CancellationToken cancellationToken)
    {
        var normalizedModel = request.Model?.Trim();
        if (string.IsNullOrWhiteSpace(normalizedModel))
            throw new ValidationException("Model is required.");

        var normalizedBrand = request.Brand?.Trim();
        if (string.IsNullOrWhiteSpace(normalizedBrand))
            throw new ValidationException("Brand is required.");

        var yearOfManufacture = request.YearOfManufacture;
        if (string.IsNullOrWhiteSpace(normalizedBrand))
            throw new ValidationException("Year of manufacture is required.");

        var vehicleCategory = request.VehicleCategoryId;
        if (string.IsNullOrWhiteSpace(normalizedBrand))
            throw new ValidationException("Vehicle category is required.");

        var applicationUser = request.ApplicationUserId;
        if (string.IsNullOrWhiteSpace(normalizedBrand))
            throw new ValidationException("Application user is required.");


        // Check if a category with the same name already exists.
        bool exists = await context.Vehicles
            .AnyAsync(x => x.Model == normalizedModel, cancellationToken);

        if (exists)
        {
            throw new MarketConflictException("Vehicle already exists.");
        }

        var vehicle = new VehicleEntity
        {
            Model = normalizedModel,
            Brand = normalizedBrand,
            YearOfManufacture = yearOfManufacture,
            ApplicationUserId = applicationUser,
            VehicleCategoryId = vehicleCategory
        };

        context.Vehicles.Add(vehicle);
        await context.SaveChangesAsync(cancellationToken);

        return vehicle.Id;
    }
}
