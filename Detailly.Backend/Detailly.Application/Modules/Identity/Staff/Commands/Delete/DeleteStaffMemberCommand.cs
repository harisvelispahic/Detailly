namespace Detailly.Application.Modules.Identity.Staff.Commands.Delete;

public class DeleteStaffMemberCommand : IRequest<Unit>
{
    public required int Id { get; set; }
}
