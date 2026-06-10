namespace Detailly.Application.Modules.Identity.User.Commands.SetFleetStatus;

public sealed class SetFleetStatusCommandHandler(
    IAppDbContext ctx,
    IAppAuthorizationService authService)
    : IRequestHandler<SetFleetStatusCommand, Unit>
{
    public async Task<Unit> Handle(SetFleetStatusCommand request, CancellationToken ct)
    {
        authService.EnsureAdmin();

        var user = await ctx.ApplicationUsers
            .FirstOrDefaultAsync(x => x.Id == request.UserId && !x.IsDeleted, ct);

        if (user is null)
            throw new DetaillyNotFoundException("User not found.");

        user.IsFleet = request.IsFleet;
        user.TokenVersion++;
        user.ModifiedAtUtc = DateTime.UtcNow;

        await ctx.SaveChangesAsync(ct);

        return Unit.Value;
    }
}
