using Detailly.Domain.Entities.Booking;

namespace Detailly.Application.Modules.Booking.ServicePackages.Commands.Create;

public class CreateServicePackageCommandHandler(IAppDbContext context)
    : IRequestHandler<CreateServicePackageCommand, int>
{
    public async Task<int> Handle(CreateServicePackageCommand request, CancellationToken ct)
    {
        await using var transaction = await context.Database.BeginTransactionAsync(ct);

        var package = new ServicePackageEntity
        {
            Name = request.Name.Trim(),
            Description = request.Description?.Trim(),
            Price = request.Price,
            CreatedAtUtc = DateTime.UtcNow
        };

        context.ServicePackages.Add(package);
        await context.SaveChangesAsync(ct);

        var distinctIds = (request.ServicePackageItemIds ?? new List<int>()).Distinct().ToList();

        if (distinctIds.Count > 0)
        {
            var sortedNewIds = distinctIds.OrderBy(x => x).ToList();

            var existingAssignments = await context.ServicePackageItemAssignments
                .Where(a => !a.IsDeleted && !a.ServicePackage.IsDeleted)
                .Select(a => new { a.ServicePackageId, a.ServicePackageItemId })
                .ToListAsync(ct);

            bool isDuplicate = existingAssignments
                .GroupBy(a => a.ServicePackageId)
                .Any(g => g.Select(a => a.ServicePackageItemId).OrderBy(x => x).SequenceEqual(sortedNewIds));

            if (isDuplicate)
                throw new DetaillyBusinessRuleException("SERVICE_PACKAGE_DUPLICATE_ITEMS",
                    "A service package with the exact same set of items already exists.");

            // Load full items so we can compute totals
            var items = await context.ServicePackageItems
                .Where(x => distinctIds.Contains(x.Id) && !x.IsDeleted /* && x.IsActive */)
                .ToListAsync(ct);

            if (items.Count != distinctIds.Count)
                throw new DetaillyBusinessRuleException("SERVICE_PACKAGE_ITEM_INVALID",
                    "One or more ServicePackageItem ids are invalid.");

            // Create assignments
            var assignments = distinctIds.Select(itemId => new ServicePackageItemAssignmentEntity
            {
                ServicePackageId = package.Id,
                ServicePackageItemId = itemId,
                CreatedAtUtc = DateTime.UtcNow
            });

            context.ServicePackageItemAssignments.AddRange(assignments);

            // Cache totals on package
            package.BaseDurationMinutes = items.Sum(i => i.DurationMinutes);
            package.BaseRequiredEmployees = items.Max(i => i.RequiredEmployees);

            await context.SaveChangesAsync(ct);
        }

        await transaction.CommitAsync(ct);
        return package.Id;
    }
}
