namespace Detailly.Application.Modules.Booking.ServicePackageItems.Commands.Delete;

public class DeleteServicePackageItemCommandHandler(IAppDbContext context)
    : IRequestHandler<DeleteServicePackageItemCommand, Unit>
{
    public async Task<Unit> Handle(DeleteServicePackageItemCommand request, CancellationToken ct)
    {
        await using var transaction = await context.Database.BeginTransactionAsync(ct);

        var item = await context.ServicePackageItems
            .FirstOrDefaultAsync(x => x.Id == request.Id, ct);

        if (item is null || item.IsDeleted)
            throw new DetaillyNotFoundException("Service package item not found.");

        var inUse = await context.ServicePackageItemAssignments
            .AnyAsync(a => a.ServicePackageItemId == request.Id && !a.IsDeleted && !a.ServicePackage.IsDeleted, ct);

        if (inUse)
            throw new DetaillyBusinessRuleException("SERVICE_PACKAGE_ITEM_IN_USE",
                "Cannot delete this item because it is assigned to one or more service packages.");

        item.IsDeleted = true;
        item.ModifiedAtUtc = DateTime.UtcNow;

        await context.SaveChangesAsync(ct);
        await transaction.CommitAsync(ct);

        return Unit.Value;
    }
}
