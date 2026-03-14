namespace Detailly.Application.Modules.Identity.User.Commands.ChangePassword;
public class ChangePasswordCommand : IRequest<Unit>
{
    public required string CurrentPassword { get; set; }
    public required string NewPassword { get; set; }
}
