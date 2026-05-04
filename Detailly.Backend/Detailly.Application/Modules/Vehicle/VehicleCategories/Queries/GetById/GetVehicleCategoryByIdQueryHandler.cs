using Detailly.Application.Common.Exceptions;

namespace Detailly.Application.Modules.Vehicle.VehicleCategories.Queries.GetById;

public class GetVehicleCategoryByIdQueryHandler(IAppDbContext context)
    : IRequestHandler<GetVehicleCategoryByIdQuery, GetVehicleCategoryByIdQueryDto>
{
    public async Task<GetVehicleCategoryByIdQueryDto> Handle(
        GetVehicleCategoryByIdQuery request,
        CancellationToken ct)
    {
        var dto = await context.VehicleCategories
            .AsNoTracking()
            .Where(c => c.Id == request.Id)
            .Select(c => new GetVehicleCategoryByIdQueryDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                BasePriceMultiplier = c.BasePriceMultiplier
            })
            .FirstOrDefaultAsync(ct);

        if (dto is null)
            throw new DetaillyNotFoundException("Vehicle category not found.");

        return dto;
    }
}
