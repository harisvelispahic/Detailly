namespace Detailly.Application.Modules.Vehicle.Vehicles.Queries.GetById;

public class GetVehicleByIdQueryHandler(IAppDbContext context, IAppCurrentUser appCurrentUser) : IRequestHandler<GetVehicleByIdQuery, GetVehicleByIdQueryDto>
{
    public async Task<GetVehicleByIdQueryDto> Handle(GetVehicleByIdQuery request, CancellationToken ct)
    {
        if (!appCurrentUser.IsAuthenticated || appCurrentUser.ApplicationUserId is null)
            throw new DetaillyUnauthorizedException("User is not authenticated.");

        var vehicle = await context.Vehicles
            .Where(c => c.Id == request.Id)
            .Select(x => new
            {
                x.ApplicationUserId,
                Dto = new GetVehicleByIdQueryDto
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
                }
            })
            .FirstOrDefaultAsync(ct);

        if (vehicle == null)
        {
            throw new DetaillyNotFoundException($"Vehicle with Id {request.Id} not found.");
        }

        // Authorization: allow if admin, otherwise owner only
        if (!appCurrentUser.IsAdmin && vehicle.ApplicationUserId != appCurrentUser.ApplicationUserId)
            throw new DetaillyForbiddenException("You are not allowed to view this vehicle.");

        return vehicle.Dto;
    }
}
