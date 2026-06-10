using Detailly.Application.Abstractions;

namespace Detailly.Application.Modules.Identity.User.Queries.List;

public class ListUsersQueryHandler(
    IAppDbContext ctx,
    IAppAuthorizationService authService)
    : IRequestHandler<ListUsersQuery, PageResult<ListUsersQueryDto>>
{
    public async Task<PageResult<ListUsersQueryDto>> Handle(
        ListUsersQuery request, CancellationToken ct)
    {
        authService.EnsureAdmin();

        var q = ctx.ApplicationUsers
            .AsNoTracking()
            .Where(x => !x.IsAdmin && !x.IsManager && !x.IsEmployee && !x.IsDeleted);

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
                CompanyName = x.CompanyName,
                IsFleet = x.IsFleet
            });

        return await PageResult<ListUsersQueryDto>.FromQueryableAsync(projectedQuery, request.Paging, ct);
    }
}
