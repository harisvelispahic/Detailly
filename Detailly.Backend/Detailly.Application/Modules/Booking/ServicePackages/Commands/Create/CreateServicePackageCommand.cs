namespace Detailly.Application.Modules.Booking.ServicePackages.Commands.Create;

public class CreateServicePackageCommand : IRequest<int>
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public required decimal Price { get; set; }
    public required int EstimatedDurationHours { get; set; }

    // Items that build this package
    public List<int>? ItemIds { get; set; }
}
