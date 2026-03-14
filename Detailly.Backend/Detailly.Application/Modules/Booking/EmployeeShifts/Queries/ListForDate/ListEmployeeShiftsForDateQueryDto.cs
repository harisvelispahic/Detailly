using Detailly.Domain.Common.Enums;

namespace Detailly.Application.Modules.Booking.EmployeeShifts.Queries.ListForDate;

public sealed class ListEmployeeShiftsForDateQueryDto
{
    public required int Id { get; set; }
    public required int EmployeeId { get; set; }
    public required string EmployeeName { get; set; }
    public required int ShopLocationId { get; set; }
    public required EmployeeWorkMode EmployeeWorkMode { get; set; }
    public required DateTime StartUtc { get; set; }
    public required DateTime EndUtc { get; set; }
}