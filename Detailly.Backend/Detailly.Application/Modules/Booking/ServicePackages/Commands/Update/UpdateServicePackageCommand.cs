namespace Detailly.Application.Modules.Booking.ServicePackages.Commands.Update;

public class UpdateServicePackageCommand : IRequest<Unit>
{
    [JsonIgnore]
    public int Id { get; set; }

    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal? Price { get; set; }

    public List<int>? ServicePackageItemIds { get; set; }
}
