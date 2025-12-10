
namespace Detailly.Application.Modules.Identity.User.Commands.Update;
public class UpdateUserCommand : IRequest<Unit>
{
    public required int Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Username { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? CompanyName { get; set; }
    public UpdateUserCommandAddress? Address { get; set; }
    public UpdateUserCommandImage? Image { get; set; }

}

public class UpdateUserCommandAddress
{
    public string? Street { get; set; }
    public string? City { get; set; }
    public string? Region { get; set; }
    public string? PostalCode { get; set; }
    public string? Country { get; set; }

    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
}

public class UpdateUserCommandImage
{
    public string? ImageUrl { get; set; }
}