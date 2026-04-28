namespace Detailly.Application.Modules.Booking.EmployeeShifts.Queries.ListMine;

public sealed class ListMyShiftsQuery : IRequest<List<ListMyShiftsQueryDto>>
{
    public int Days { get; set; } = 7;
}
