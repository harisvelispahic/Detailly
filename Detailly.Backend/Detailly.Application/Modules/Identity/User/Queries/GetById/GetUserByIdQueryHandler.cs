namespace Detailly.Application.Modules.Identity.User.Queries.GetById;

public class GetUserByIdQueryHandler(
    IAppDbContext context,
    IAppCurrentUser appCurrentUser)
    : IRequestHandler<GetUserByIdQuery, GetUserByIdQueryDto>
{
    public async Task<GetUserByIdQueryDto> Handle(GetUserByIdQuery request, CancellationToken ct)
    {
        if (!appCurrentUser.IsAuthenticated || appCurrentUser.ApplicationUserId is null)
            throw new DetaillyUnauthorizedException("User is not authenticated.");

        var isStaff = appCurrentUser.IsAdmin || appCurrentUser.IsManager;
        var currentUserId = appCurrentUser.ApplicationUserId.Value;

        // Only staff or the user themself can view the user details
        if (!isStaff && request.Id != currentUserId)
            throw new DetaillyForbiddenException("You are not allowed to view this user.");

        var dto = await context.ApplicationUsers
            .AsNoTracking()
            .Where(u => u.Id == request.Id)
            .Select(u => new GetUserByIdQueryDto
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Username = u.Username,
                Email = u.Email,
                Phone = u.Phone,
                CompanyName = u.CompanyName,
                IsOAuthUser = u.ExternalLogins.Any()
            })
            .FirstOrDefaultAsync(ct);

        if (dto == null)
            throw new DetaillyNotFoundException($"User with Id {request.Id} not found.");

        return dto;
    }
}
