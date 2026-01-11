namespace Detailly.Application.Modules.Booking.ServicePackages.Commands.Update;

public class UpdateServicePackageCommand : IRequest<Unit>
{
    public required int Id { get; set; }

    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal? Price { get; set; }
    public int? EstimatedDurationHours { get; set; }

    // If provided -> replace assignments
    public List<int>? ItemIds { get; set; }
}
