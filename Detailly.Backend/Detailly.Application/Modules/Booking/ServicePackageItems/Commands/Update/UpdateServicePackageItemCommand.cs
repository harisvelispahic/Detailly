namespace Detailly.Application.Modules.Booking.ServicePackageItems.Commands.Update;

public class UpdateServicePackageItemCommand : IRequest<Unit>
{
    [JsonIgnore]
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal? Price { get; set; }
    public int? DurationMinutes { get; set; }
    public int? RequiredEmployees { get; set; }
    public bool? IsAddon { get; set; }
    public bool? IsActive { get; set; }
}
