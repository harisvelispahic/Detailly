
using Detailly.Domain.Entities.Shared;

namespace Detailly.Application.Modules.Identity.User.Queries.List;
public class ListUsersQueryDto
{
    public required int Id { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required string Username { get; init; }
    public required string Email { get; init; }
    public string? CompanyName { get; init; }
    public required AddressEntity Address { get; init; }
}
