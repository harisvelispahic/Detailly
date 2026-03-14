namespace Detailly.Application.Modules.Identity.User.Commands.Delete;
public class DeleteUserCommand : IRequest<Unit>
{
    public required int Id { get; set; }
}
