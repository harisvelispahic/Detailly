namespace Detailly.Application.Modules.Booking.ServicePackageItems.Commands.Create;

public class CreateServicePackageItemCommand : IRequest<int>
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public required decimal Price { get; set; }
    public required int DurationMinutes { get; set; }
    public int RequiredEmployees { get; set; } = 1;
    public bool IsAddon { get; set; } = false;
}
