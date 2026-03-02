using Detailly.Domain.Common.Enums;
using MediatR;

namespace Detailly.Application.Modules.Booking.EmployeeShifts.Commands.Create;

public sealed class CreateEmployeeShiftCommand : IRequest<int>
{
    public required int EmployeeId { get; set; }
    public required int ShopLocationId { get; set; }
    public required EmployeeWorkMode EmployeeWorkMode { get; set; }
    public required DateTime StartUtc { get; set; }
    public required DateTime EndUtc { get; set; }
}