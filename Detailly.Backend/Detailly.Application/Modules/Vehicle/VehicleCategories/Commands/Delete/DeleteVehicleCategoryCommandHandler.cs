using Detailly.Application.Common.Exceptions;

namespace Detailly.Application.Modules.Vehicle.VehicleCategories.Commands.Delete;

public class DeleteVehicleCategoryCommandHandler(IAppDbContext context)
    : IRequestHandler<DeleteVehicleCategoryCommand, Unit>
{
    public async Task<Unit> Handle(DeleteVehicleCategoryCommand request, CancellationToken ct)
    {
        var category = await context.VehicleCategories
            .Include(c => c.Vehicles)
            .FirstOrDefaultAsync(c => c.Id == request.Id, ct);

        if (category is null)
            throw new DetaillyNotFoundException("Vehicle category not found.");

        if (category.Vehicles.Any())
            throw new DetaillyBusinessRuleException(
                "CATEGORY_HAS_VEHICLES",
                "Cannot delete a vehicle category that has vehicles assigned to it.");

        category.IsDeleted = true;
        category.ModifiedAtUtc = DateTime.UtcNow;

        await context.SaveChangesAsync(ct);
        return Unit.Value;
    }
}
