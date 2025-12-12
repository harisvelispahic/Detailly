
namespace Detailly.Application.Modules.Identity.User.Queries.List;
public class ListUsersQueryHandler(IAppDbContext ctx)
        : IRequestHandler<ListUsersQuery, PageResult<ListUsersQueryDto>>
{
    public async Task<PageResult<ListUsersQueryDto>> Handle(
        ListUsersQuery request, CancellationToken ct)
    {
        var q = ctx.ApplicationUsers.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            q = q.Where(x => 
                x.FirstName.Contains(request.Search) ||
                x.LastName.Contains(request.Search) ||
                x.Username.Contains(request.Search) ||
                x.Email.Contains(request.Search));
        }

        var projectedQuery = 
            q.OrderBy(x => x.LastName)
            .ThenBy(x => x.FirstName)
            .Select(x => new ListUsersQueryDto
            {
                Id = x.Id,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Username = x.Username,
                Email = x.Email,
                CompanyName = x.CompanyName,
                Address = x.Address!
            });
        return await PageResult<ListUsersQueryDto>.FromQueryableAsync(projectedQuery, request.Paging, ct);
    }
}
