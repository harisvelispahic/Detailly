using Detailly.Domain.Entities.Vehicle;

namespace Detailly.Application.Modules.Vehicle.VehicleCategories.Commands.Create;

public class CreateVehicleCategoryCommandHandler(IAppDbContext context)
    : IRequestHandler<CreateVehicleCategoryCommand, int>
{
    public async Task<int> Handle(CreateVehicleCategoryCommand request, CancellationToken ct)
    {
        var entity = new VehicleCategoryEntity
        {
            Name = request.Name,
            Description = request.Description,
            BasePriceMultiplier = request.BasePriceMultiplier
        };

        context.VehicleCategories.Add(entity);
        await context.SaveChangesAsync(ct);

        return entity.Id;
    }
}
