using Detailly.Domain.Common.Enums;

namespace Detailly.Application.Modules.Booking.EmployeeShifts.Commands.Update;

public sealed class UpdateEmployeeShiftCommand : IRequest<Unit>
{
    public required int Id { get; set; } // route id
    public required int EmployeeId { get; set; }
    public required int ShopLocationId { get; set; }
    public required EmployeeWorkMode EmployeeWorkMode { get; set; }
    public required DateTime StartUtc { get; set; }
    public required DateTime EndUtc { get; set; }
}