using Detailly.Domain.Common.Enums;

namespace Detailly.Application.Modules.Booking.EmployeeShifts.Queries.ExportShifts;

public class ExportShiftsQuery : IRequest<List<ExportShiftsQueryDto>>
{
    public DateTime StartDateUtc { get; set; }
    public DateTime EndDateUtc { get; set; }
    public int ShopLocationId { get; set; }
    public EmployeeWorkMode? EmployeeWorkMode { get; set; }
}
