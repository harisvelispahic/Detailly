using System.Text.Json.Serialization;

namespace Detailly.Application.Modules.Vehicle.VehicleCategories.Commands.Update;

public class UpdateVehicleCategoryCommand : IRequest<Unit>
{
    [JsonIgnore]
    public int Id { get; set; }

    public string? Name { get; init; }
    public string? Description { get; init; }
    public decimal? BasePriceMultiplier { get; init; }
}
