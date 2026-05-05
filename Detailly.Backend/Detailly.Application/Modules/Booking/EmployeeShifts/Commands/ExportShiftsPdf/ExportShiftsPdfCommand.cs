using Detailly.Domain.Common.Enums;

namespace Detailly.Application.Modules.Booking.EmployeeShifts.Commands.ExportShiftsPdf;

public sealed class ExportShiftsPdfCommand : IRequest<byte[]>
{
    public DateTime StartDateUtc { get; set; }
    public DateTime EndDateUtc { get; set; }
    public int ShopLocationId { get; set; }
    public EmployeeWorkMode? EmployeeWorkMode { get; set; }
}
