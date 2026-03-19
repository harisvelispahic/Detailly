using Detailly.Domain.Common.Enums;

namespace Detailly.Application.Modules.Booking.EmployeeShifts.Commands.Update;

public sealed class UpdateEmployeeShiftCommand : IRequest<Unit>
{
    [JsonIgnore]
    public int Id { get; set; } // route id

    // Made optional for selective updates
    public int? EmployeeId { get; set; }
    public int? ShopLocationId { get; set; }
    public EmployeeWorkMode? EmployeeWorkMode { get; set; }
    public DateTime? StartUtc { get; set; }
    public DateTime? EndUtc { get; set; }
}