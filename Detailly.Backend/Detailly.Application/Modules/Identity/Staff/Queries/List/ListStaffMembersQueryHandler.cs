namespace Detailly.Application.Modules.Identity.Staff.Queries.List;

public sealed class ListStaffMembersQueryHandler(
    IAppDbContext ctx,
    IAppCurrentUser appCurrentUser)
    : IRequestHandler<ListStaffMembersQuery, PageResult<ListStaffMembersQueryDto>>
{
    public async Task<PageResult<ListStaffMembersQueryDto>> Handle(
        ListStaffMembersQuery request, CancellationToken ct)
    {
        if (!appCurrentUser.IsAuthenticated)
            throw new DetaillyUnauthorizedException("User is not authenticated.");

        if (!appCurrentUser.IsAdmin && !appCurrentUser.IsManager)
            throw new DetaillyForbiddenException("Only admins and managers can list staff members.");

        var q = ctx.ApplicationUsers
            .AsNoTracking()
            .Where(x => !x.IsDeleted && (x.IsEmployee || x.IsManager));

        if (request.IsManager.HasValue)
        {
            q = request.IsManager.Value
                ? q.Where(x => x.IsManager)
                : q.Where(x => x.IsEmployee && !x.IsManager);
        }

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var s = request.Search.Trim();
            q = q.Where(x =>
                x.FirstName.Contains(s) ||
                x.LastName.Contains(s) ||
                x.Username.Contains(s) ||
                x.Email.Contains(s));
        }

        var projected = q
            .OrderBy(x => x.LastName)
            .ThenBy(x => x.FirstName)
            .Select(x => new ListStaffMembersQueryDto
            {
                Id = x.Id,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Username = x.Username,
                Email = x.Email,
                Phone = x.Phone,
                IsManager = x.IsManager
            });

        return await PageResult<ListStaffMembersQueryDto>.FromQueryableAsync(projected, request.Paging, ct);
    }
}
