using Detailly.Domain.Common.Enums;

namespace Detailly.Application.Modules.Booking.EmployeeShifts.Queries.ListForDate;

public sealed class ListEmployeeShiftsForDateQuery : BasePagedQuery<ListEmployeeShiftsForDateQueryDto>
{
    public required DateTime DateUtc { get; set; }           // only Date part used
    public required int ShopLocationId { get; set; }
    public EmployeeWorkMode? EmployeeWorkMode { get; set; }  // optional filter
}