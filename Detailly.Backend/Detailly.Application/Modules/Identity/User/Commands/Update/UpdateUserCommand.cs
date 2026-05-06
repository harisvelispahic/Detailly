namespace Detailly.Application.Modules.Identity.User.Commands.Update;
public class UpdateUserCommand : IRequest<Unit>
{
    [JsonIgnore]
    public required int Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Username { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? CompanyName { get; set; }
}