
namespace Detailly.Application.Modules.Vehicle.Vehicles.Queries.GetById;

public class GetVehicleByIdQuery : IRequest<GetVehicleByIdQueryDto>
{
    public int Id { get; set; }
}
