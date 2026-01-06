using Microsoft.EntityFrameworkCore;
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
            EstimatedDurationHours = request.EstimatedDurationHours,
            CreatedAtUtc = DateTime.UtcNow
        };

        context.ServicePackages.Add(package);
        await context.SaveChangesAsync(ct); // da dobijemo package.Id

        if (request.ItemIds is not null && request.ItemIds.Count > 0)
        {
            var distinctIds = request.ItemIds.Distinct().ToList();

            // Validate items exist (and are not deleted)
            var existingItemIds = await context.ServicePackageItems
                .Where(x => distinctIds.Contains(x.Id) && !x.IsDeleted)
                .Select(x => x.Id)
                .ToListAsync(ct);

            if (existingItemIds.Count != distinctIds.Count)
                throw new DetaillyBusinessRuleException("SERVICE_PACKAGE_ITEM_INVALID", "One or more ServicePackageItem ids are invalid.");

            var assignments = distinctIds.Select(itemId => new ServicePackageItemAssignmentEntity
            {
                ServicePackageId = package.Id,
                ServicePackageItemId = itemId,
                CreatedAtUtc = DateTime.UtcNow
            });

            context.ServicePackageItemAssignments.AddRange(assignments);
            await context.SaveChangesAsync(ct);
        }

        await transaction.CommitAsync(ct);
        return package.Id;
    }
}
