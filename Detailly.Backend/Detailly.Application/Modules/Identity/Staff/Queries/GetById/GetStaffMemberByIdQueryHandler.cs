namespace Detailly.Application.Modules.Identity.Staff.Queries.GetById;

public sealed class GetStaffMemberByIdQueryHandler(
    IAppDbContext context,
    IAppCurrentUser appCurrentUser)
    : IRequestHandler<GetStaffMemberByIdQuery, GetStaffMemberByIdQueryDto>
{
    public async Task<GetStaffMemberByIdQueryDto> Handle(GetStaffMemberByIdQuery request, CancellationToken ct)
    {
        if (!appCurrentUser.IsAuthenticated)
            throw new DetaillyUnauthorizedException("User is not authenticated.");

        if (!appCurrentUser.IsAdmin && !appCurrentUser.IsManager)
            throw new DetaillyForbiddenException("Only admins and managers can view staff members.");

        var dto = await context.ApplicationUsers
            .AsNoTracking()
            .Where(x => x.Id == request.Id && !x.IsDeleted && (x.IsEmployee || x.IsManager))
            .Select(x => new GetStaffMemberByIdQueryDto
            {
                Id = x.Id,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Username = x.Username,
                Email = x.Email,
                Phone = x.Phone,
                IsManager = x.IsManager
            })
            .FirstOrDefaultAsync(ct);

        if (dto is null)
            throw new DetaillyNotFoundException($"Staff member with Id {request.Id} not found.");

        return dto;
    }
}
