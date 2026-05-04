using System.Text.Json.Serialization;

namespace Detailly.Application.Modules.Vehicle.VehicleCategories.Commands.Delete;

public class DeleteVehicleCategoryCommand : IRequest<Unit>
{
    [JsonIgnore]
    public int Id { get; set; }
}
