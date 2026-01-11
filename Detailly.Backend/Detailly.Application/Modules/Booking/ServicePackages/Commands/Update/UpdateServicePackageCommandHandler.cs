using Microsoft.EntityFrameworkCore;
using Detailly.Domain.Entities.Booking;

namespace Detailly.Application.Modules.Booking.ServicePackages.Commands.Update;

public class UpdateServicePackageCommandHandler(IAppDbContext context)
    : IRequestHandler<UpdateServicePackageCommand, Unit>
{
    public async Task<Unit> Handle(UpdateServicePackageCommand request, CancellationToken ct)
    {
        await using var transaction = await context.Database.BeginTransactionAsync(ct);

        var package = await context.ServicePackages
            .FirstOrDefaultAsync(x => x.Id == request.Id, ct);

        if (package is null || package.IsDeleted)
            throw new DetaillyNotFoundException("Service package not found.");

        // Partial update fields
        if (request.Name is not null)
            package.Name = request.Name.Trim();

        if (request.Description is not null)
            package.Description = request.Description.Trim();

        if (request.Price.HasValue)
            package.Price = request.Price.Value;

        if (request.EstimatedDurationHours.HasValue)
            package.EstimatedDurationHours = request.EstimatedDurationHours.Value;

        // Replace items if provided
        if (request.ItemIds is not null)
        {
            var distinctIds = request.ItemIds.Distinct().ToList();

            // Soft delete existing assignments
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
                // Validate items exist
                var existingItemIds = await context.ServicePackageItems
                    .Where(x => distinctIds.Contains(x.Id) && !x.IsDeleted)
                    .Select(x => x.Id)
                    .ToListAsync(ct);

                if (existingItemIds.Count != distinctIds.Count)
                    throw new DetaillyBusinessRuleException("SERVICE_PACKAGE_ITEM_INVALID","One or more ServicePackageItem ids are invalid.");

                var newAssignments = distinctIds.Select(itemId => new ServicePackageItemAssignmentEntity
                {
                    ServicePackageId = package.Id,
                    ServicePackageItemId = itemId,
                    CreatedAtUtc = DateTime.UtcNow
                });

                context.ServicePackageItemAssignments.AddRange(newAssignments);
            }
        }

        package.ModifiedAtUtc = DateTime.UtcNow;

        await context.SaveChangesAsync(ct);
        await transaction.CommitAsync(ct);

        return Unit.Value;
    }
}
