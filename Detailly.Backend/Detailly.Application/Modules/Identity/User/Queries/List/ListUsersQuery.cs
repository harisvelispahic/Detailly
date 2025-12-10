
namespace Detailly.Application.Modules.Identity.User.Queries.List;
public class ListUsersQuery : BasePagedQuery<ListUsersQueryDto>
{
    public string? Search { get; init; }
}
