namespace Detailly.Application.Modules.Booking.ServicePackages.Queries.GetAvailableAddons;

public sealed class GetAvailableAddonsQueryDto
{
    public required int Id { get; set; }
    public required string Name { get; set; }
    public required decimal Price { get; set; }
    public required int DurationMinutes { get; set; }
    public required int RequiredEmployees { get; set; }
    public string? Description { get; set; }
}