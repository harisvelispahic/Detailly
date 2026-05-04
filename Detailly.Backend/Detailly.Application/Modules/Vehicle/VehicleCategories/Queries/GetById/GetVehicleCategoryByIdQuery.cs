namespace Detailly.Application.Modules.Vehicle.VehicleCategories.Queries.GetById;

public class GetVehicleCategoryByIdQuery : IRequest<GetVehicleCategoryByIdQueryDto>
{
    public required int Id { get; init; }
}
