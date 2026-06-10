namespace Detailly.Application.Modules.Identity.User.Commands.SetFleetStatus;

public sealed class SetFleetStatusCommand : IRequest<Unit>
{
    public required int UserId { get; set; }
    public required bool IsFleet { get; set; }
}
