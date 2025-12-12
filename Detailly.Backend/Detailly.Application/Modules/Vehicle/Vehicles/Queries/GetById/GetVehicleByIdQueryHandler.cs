
namespace Detailly.Application.Modules.Vehicle.Vehicles.Queries.GetById;

public class GetVehicleByIdQueryHandler(IAppDbContext context) : IRequestHandler<GetVehicleByIdQuery, GetVehicleByIdQueryDto>
{
    public async Task<GetVehicleByIdQueryDto> Handle(GetVehicleByIdQuery request, CancellationToken ct)
    {
        var vehicle = await context.Vehicles
            .Where(c => c.Id == request.Id)
            .Select(x => new GetVehicleByIdQueryDto
            {
                Id = x.Id,
                Brand = x.Brand,
                Model = x.Model,
                YearOfManufacture = x.YearOfManufacture,
                LicencePlate = x.LicencePlate,
                Notes = x.Notes,
                VehicleCategory = new GetVehicleByIdQueryDtoVehicleCategory
                {
                    Name = x.VehicleCategory.Name
                },
            })
            .FirstOrDefaultAsync(ct);

        if (vehicle == null)
        {
            throw new DetaillyNotFoundException($"Vehicle with Id {request.Id} not found.");
        }

        return vehicle;
    }
}
