namespace Detailly.Application.Modules.Booking.Locations.Commands.ToggleStatus;

public sealed class ToggleLocationStatusCommandHandler(IAppDbContext context, IAppCurrentUser appCurrentUser)
    : IRequestHandler<ToggleLocationStatusCommand, Unit>
{
    public async Task<Unit> Handle(ToggleLocationStatusCommand request, CancellationToken ct)
    {
        if (!appCurrentUser.IsAuthenticated)
            throw new DetaillyBusinessRuleException("AUTH_REQUIRED", "Authentication required.");

        if (!appCurrentUser.IsAdmin)
            throw new DetaillyBusinessRuleException("FORBIDDEN", "Only admins can change location status.");

        var location = await context.Locations
            .FirstOrDefaultAsync(l => l.Id == request.Id && !l.IsDeleted, ct);

        if (location is null)
            throw new DetaillyNotFoundException("Location not found.");

        location.IsTemporarilyClosed = !location.IsTemporarilyClosed;
        location.ModifiedAtUtc = DateTime.UtcNow;

        await context.SaveChangesAsync(ct);
        return Unit.Value;
    }
}
