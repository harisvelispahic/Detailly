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

        var inActivePackage = await context.ServicePackageItemAssignments
            .AnyAsync(a => a.ServicePackageItemId == request.Id && !a.IsDeleted && !a.ServicePackage.IsDeleted, ct);

        if (inActivePackage)
            throw new DetaillyBusinessRuleException("SERVICE_PACKAGE_ITEM_IN_USE",
                "Cannot delete this item because it is assigned to one or more service packages.");

        var inBooking = await context.BookingItems
            .AnyAsync(bi => bi.ServicePackageItemId == request.Id, ct);

        if (inBooking)
            throw new DetaillyBusinessRuleException("SERVICE_PACKAGE_ITEM_IN_BOOKING",
                "Cannot delete this item because it has been used in a booking.");

        item.IsDeleted = true;
        item.ModifiedAtUtc = DateTime.UtcNow;

        await context.SaveChangesAsync(ct);
        await transaction.CommitAsync(ct);

        return Unit.Value;
    }
}
