using Detailly.Application.Common.Exceptions;

namespace Detailly.Application.Modules.Vehicle.VehicleCategories.Commands.Update;

public class UpdateVehicleCategoryCommandHandler(IAppDbContext context)
    : IRequestHandler<UpdateVehicleCategoryCommand, Unit>
{
    public async Task<Unit> Handle(UpdateVehicleCategoryCommand request, CancellationToken ct)
    {
        var category = await context.VehicleCategories
            .FirstOrDefaultAsync(c => c.Id == request.Id, ct);

        if (category is null)
            throw new DetaillyNotFoundException("Vehicle category not found.");

        if (request.Name is not null) category.Name = request.Name;
        if (request.Description is not null) category.Description = request.Description;
        if (request.BasePriceMultiplier is not null) category.BasePriceMultiplier = request.BasePriceMultiplier.Value;

        category.ModifiedAtUtc = DateTime.UtcNow;

        await context.SaveChangesAsync(ct);
        return Unit.Value;
    }
}
