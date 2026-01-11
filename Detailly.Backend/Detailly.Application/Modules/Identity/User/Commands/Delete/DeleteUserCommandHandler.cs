namespace Detailly.Application.Modules.Identity.User.Commands.Delete;
public class DeleteUserCommandHandler(IAppDbContext context, IAppCurrentUser appCurrentUser)
        : IRequestHandler<DeleteUserCommand, Unit>
{
    public async Task<Unit> Handle(DeleteUserCommand request, CancellationToken ct)
    {
        //if (!appCurrentUser.IsAdmin)
        //    throw new DetaillyUnauthorizedException("User is not authorized to do this action.");

        var user = await context.ApplicationUsers
            .FirstOrDefaultAsync(x => x.Id == request.Id, ct);

        if (user is null)
            throw new DetaillyNotFoundException("User was not found.");

        user.IsDeleted = true;
        await context.SaveChangesAsync(ct);

        return Unit.Value;
    }
}