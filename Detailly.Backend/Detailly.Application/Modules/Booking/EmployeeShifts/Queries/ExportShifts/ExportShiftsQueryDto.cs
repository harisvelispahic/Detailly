using Detailly.Domain.Common.Enums;

namespace Detailly.Application.Modules.Booking.EmployeeShifts.Queries.ExportShifts;

public class ExportShiftsQueryDto
{
    public int Id { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public EmployeeWorkMode EmployeeWorkMode { get; set; }
    public DateTime StartUtc { get; set; }
    public DateTime EndUtc { get; set; }
    public string LocationName { get; set; } = string.Empty;
}
