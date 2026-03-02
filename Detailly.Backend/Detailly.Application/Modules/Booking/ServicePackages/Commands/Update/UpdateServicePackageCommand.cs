namespace Detailly.Application.Modules.Booking.ServicePackages.Commands.Update;

public class UpdateServicePackageCommand : IRequest<Unit>
{
    public required int Id { get; set; }

    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal? Price { get; set; }

    public List<int>? ServicePackageItemIds { get; set; }
}
