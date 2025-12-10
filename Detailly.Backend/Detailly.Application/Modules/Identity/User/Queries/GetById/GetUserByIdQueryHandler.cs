namespace Detailly.Application.Modules.Identity.User.Queries.GetById;
public class GetUserByIdQueryHandler(IAppDbContext context) 
    : IRequestHandler<GetUserByIdQuery, GetUserByIdQueryDto>
{
    public async Task<GetUserByIdQueryDto> Handle(GetUserByIdQuery request, CancellationToken ct)
    {
        var user = await context.ApplicationUsers
            .Where(u => u.Id == request.Id)
            .Select(u => new GetUserByIdQueryDto
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Username = u.Username,
                Email = u.Email,
                CompanyName = u.CompanyName,
                Address = u.Address
            })
            .FirstOrDefaultAsync(ct);

        if (user == null)
        {
            throw new DetaillyNotFoundException($"User with Id {request.Id} not found.");
        }

        return user;
    }
}
