
namespace Detailly.Application.Modules.Booking.Locations.Commands.Delete;

public sealed class DeleteLocationCommandHandler(IAppDbContext context, IAppCurrentUser appCurrentUser)
    : IRequestHandler<DeleteLocationCommand, Unit>
{
    public async Task<Unit> Handle(DeleteLocationCommand request, CancellationToken ct)
    {
        var now = DateTime.UtcNow;

        if (!appCurrentUser.IsAuthenticated)
            throw new DetaillyBusinessRuleException("AUTH_REQUIRED", "Authentication required.");

        if (!appCurrentUser.IsAdmin && !appCurrentUser.IsManager)
            throw new DetaillyBusinessRuleException("FORBIDDEN", "Only admin/manager can manage locations.");

        var location = await context.Locations
            .FirstOrDefaultAsync(l => l.Id == request.Id && !l.IsDeleted, ct);

        if (location is null)
            return Unit.Value; // idempotent

        location.IsDeleted = true;
        location.ModifiedAtUtc = now;

        await context.SaveChangesAsync(ct);
        return Unit.Value;
    }
}