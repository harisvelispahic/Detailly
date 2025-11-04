
namespace Detailly.Application.Modules.Vehicle.Vehicles.Queries.GetById;

public class GetVehicleByIdQueryHandler(IAppDbContext context) : IRequestHandler<GetVehicleByIdQuery, GetVehicleByIdQueryDto>
{
    public async Task<GetVehicleByIdQueryDto> Handle(GetVehicleByIdQuery request, CancellationToken cancellationToken)
    {
        var vehicle = await context.Vehicles
            .Where(c => c.Id == request.Id)
            .Select(x => new GetVehicleByIdQueryDto
            {
                Id = x.Id,
                Model = x.Model,
                Brand = x.Brand,
                YearOfManufacture = x.YearOfManufacture
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (vehicle == null)
        {
            throw new MarketNotFoundException($"Vehicle with Id {request.Id} not found.");
        }

        return vehicle;
    }
}
