using Detailly.Domain.Entities.Booking;

namespace Detailly.Application.Modules.Booking.ServicePackages.Commands.Update;

public class UpdateServicePackageCommandHandler(IAppDbContext context)
    : IRequestHandler<UpdateServicePackageCommand, Unit>
{
    public async Task<Unit> Handle(UpdateServicePackageCommand request, CancellationToken ct)
    {
        await using var transaction = await context.Database.BeginTransactionAsync(ct);

        var package = await context.ServicePackages.FirstOrDefaultAsync(x => x.Id == request.Id, ct);

        if (package is null || package.IsDeleted)
            throw new DetaillyNotFoundException("Service package not found.");

        if (request.Name is not null) package.Name = request.Name.Trim();
        if (request.Description is not null) package.Description = request.Description.Trim();
        if (request.Price.HasValue) package.Price = request.Price.Value;

        if (request.ServicePackageItemIds is not null)
        {
            var distinctIds = request.ServicePackageItemIds.Distinct().ToList();

            if (distinctIds.Count > 0)
            {
                var sortedNewIds = distinctIds.OrderBy(x => x).ToList();

                var allOtherAssignments = await context.ServicePackageItemAssignments
                    .Where(a => !a.IsDeleted && !a.ServicePackage.IsDeleted && a.ServicePackageId != package.Id)
                    .Select(a => new { a.ServicePackageId, a.ServicePackageItemId })
                    .ToListAsync(ct);

                bool isDuplicate = allOtherAssignments
                    .GroupBy(a => a.ServicePackageId)
                    .Any(g => g.Select(a => a.ServicePackageItemId).OrderBy(x => x).SequenceEqual(sortedNewIds));

                if (isDuplicate)
                    throw new DetaillyBusinessRuleException("SERVICE_PACKAGE_DUPLICATE_ITEMS",
                        "A service package with the exact same set of items already exists.");
            }

            var existingAssignments = await context.ServicePackageItemAssignments
                .Where(x => x.ServicePackageId == package.Id && !x.IsDeleted)
                .ToListAsync(ct);

            foreach (var a in existingAssignments)
            {
                a.IsDeleted = true;
                a.ModifiedAtUtc = DateTime.UtcNow;
            }

            if (distinctIds.Count > 0)
            {
                var items = await context.ServicePackageItems
                    .Where(x => distinctIds.Contains(x.Id) && !x.IsDeleted /* && x.IsActive */)
                    .ToListAsync(ct);

                if (items.Count != distinctIds.Count)
                    throw new DetaillyBusinessRuleException("SERVICE_PACKAGE_ITEM_INVALID",
                        "One or more ServicePackageItem ids are invalid.");

                var newAssignments = distinctIds.Select(itemId => new ServicePackageItemAssignmentEntity
                {
                    ServicePackageId = package.Id,
                    ServicePackageItemId = itemId,
                    CreatedAtUtc = DateTime.UtcNow
                });

                context.ServicePackageItemAssignments.AddRange(newAssignments);

                // Recompute cached totals
                package.BaseDurationMinutes = items.Sum(i => i.DurationMinutes);
                package.BaseRequiredEmployees = items.Max(i => i.RequiredEmployees);
            }
            //else
            //{
            //    // If replacing with empty list: choose behavior
            //    package.BaseDurationMinutes = 0;
            //    package.BaseRequiredEmployees = 1;
            //}
        }

        package.ModifiedAtUtc = DateTime.UtcNow;

        await context.SaveChangesAsync(ct);
        await transaction.CommitAsync(ct);

        return Unit.Value;
    }
}
