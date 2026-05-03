namespace Detailly.Application.Modules.Identity.Staff.Commands.Update;

public class UpdateStaffMemberCommand : IRequest<Unit>
{
    [JsonIgnore]
    public required int Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Username { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
}
