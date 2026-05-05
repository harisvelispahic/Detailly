using Detailly.Application.Modules.Booking.EmployeeShifts.Queries.ExportShifts;

namespace Detailly.Application.Abstractions.PDF;

public interface IShiftsPdfGenerator
{
    byte[] Generate(List<ExportShiftsQueryDto> shifts, DateTime startDate, DateTime endDate, string locationName);
}
