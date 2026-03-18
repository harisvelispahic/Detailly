namespace Detailly.Application.Modules.Identity.User.Queries.List;

public class ListUsersQueryHandler(
    IAppDbContext ctx,
    IAppCurrentUser appCurrentUser)
    : IRequestHandler<ListUsersQuery, PageResult<ListUsersQueryDto>>
{
    public async Task<PageResult<ListUsersQueryDto>> Handle(
        ListUsersQuery request, CancellationToken ct)
    {
        // Controller already restricts listing to admins, but enforce in handler as well
        if (!appCurrentUser.IsAuthenticated || !appCurrentUser.IsAdmin)
            throw new DetaillyForbiddenException("Only admins can list users.");

        var q = ctx.ApplicationUsers.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var s = request.Search.Trim();
            q = q.Where(x =>
                x.FirstName.Contains(s) ||
                x.LastName.Contains(s) ||
                x.Username.Contains(s) ||
                x.Email.Contains(s));
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
                CompanyName = x.CompanyName
            });

        return await PageResult<ListUsersQueryDto>.FromQueryableAsync(projectedQuery, request.Paging, ct);
    }
}
