namespace Detailly.Application.Modules.Booking.ServicePackageItems.Commands.Update;

public class UpdateServicePackageItemCommandHandler(IAppDbContext context)
    : IRequestHandler<UpdateServicePackageItemCommand, Unit>
{
    public async Task<Unit> Handle(UpdateServicePackageItemCommand request, CancellationToken ct)
    {
        await using var transaction = await context.Database.BeginTransactionAsync(ct);

        var item = await context.ServicePackageItems
            .FirstOrDefaultAsync(x => x.Id == request.Id, ct);

        if (item is null || item.IsDeleted)
            throw new DetaillyNotFoundException("Service package item not found.");

        if (request.Name is not null) item.Name = request.Name.Trim();
        if (request.Description is not null) item.Description = request.Description.Trim();
        if (request.Price.HasValue) item.Price = request.Price.Value;
        if (request.IsAddon.HasValue) item.IsAddon = request.IsAddon.Value;
        if (request.IsActive.HasValue) item.IsActive = request.IsActive.Value;

        var schedulingChanged = request.DurationMinutes.HasValue || request.RequiredEmployees.HasValue;

        if (request.DurationMinutes.HasValue) item.DurationMinutes = request.DurationMinutes.Value;
        if (request.RequiredEmployees.HasValue) item.RequiredEmployees = request.RequiredEmployees.Value;

        item.ModifiedAtUtc = DateTime.UtcNow;
        await context.SaveChangesAsync(ct);

        if (schedulingChanged)
        {
            var affectedPackageIds = await context.ServicePackageItemAssignments
                .Where(a => a.ServicePackageItemId == item.Id && !a.IsDeleted && !a.ServicePackage.IsDeleted)
                .Select(a => a.ServicePackageId)
                .Distinct()
                .ToListAsync(ct);

            foreach (var packageId in affectedPackageIds)
            {
                var packageItemTotals = await context.ServicePackageItemAssignments
                    .Where(a => a.ServicePackageId == packageId && !a.IsDeleted)
                    .Select(a => new { a.ServicePackageItem.DurationMinutes, a.ServicePackageItem.RequiredEmployees })
                    .ToListAsync(ct);

                var package = await context.ServicePackages.FindAsync(new object[] { packageId }, ct);
                if (package is not null && !package.IsDeleted)
                {
                    package.BaseDurationMinutes = packageItemTotals.Sum(i => i.DurationMinutes);
                    package.BaseRequiredEmployees = packageItemTotals.Max(i => i.RequiredEmployees);
                    package.ModifiedAtUtc = DateTime.UtcNow;
                }
            }

            await context.SaveChangesAsync(ct);
        }

        await transaction.CommitAsync(ct);
        return Unit.Value;
    }
}
