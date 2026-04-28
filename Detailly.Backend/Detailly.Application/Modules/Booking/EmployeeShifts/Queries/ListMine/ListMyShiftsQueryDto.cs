using Detailly.Domain.Common.Enums;

namespace Detailly.Application.Modules.Booking.EmployeeShifts.Queries.ListMine;

public sealed class ListMyShiftsQueryDto
{
    public required int Id { get; set; }
    public required int ShopLocationId { get; set; }
    public required string ShopLocationName { get; set; }
    public required EmployeeWorkMode EmployeeWorkMode { get; set; }
    public required DateTime StartUtc { get; set; }
    public required DateTime EndUtc { get; set; }
}
