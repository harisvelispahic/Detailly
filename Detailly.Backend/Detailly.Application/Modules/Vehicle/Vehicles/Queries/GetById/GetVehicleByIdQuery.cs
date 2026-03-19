namespace Detailly.Application.Modules.Vehicle.Vehicles.Queries.GetById;

public class GetVehicleByIdQuery : IRequest<GetVehicleByIdQueryDto>
{
    [JsonIgnore]
    public int Id { get; set; }
}
