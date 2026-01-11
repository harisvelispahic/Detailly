using Microsoft.EntityFrameworkCore;
using Detailly.Application.Common.Exceptions;


namespace Detailly.Application.Modules.Booking.ServicePackages.Commands.Delete;

public class DeleteServicePackageCommandHandler(IAppDbContext context)
    : IRequestHandler<DeleteServicePackageCommand, Unit>
{
    public async Task<Unit> Handle(DeleteServicePackageCommand request, CancellationToken ct)
    {
        await using var transaction = await context.Database.BeginTransactionAsync(ct);

        var package = await context.ServicePackages
            .FirstOrDefaultAsync(x => x.Id == request.Id, ct);

        if (package is null || package.IsDeleted)
            throw new DetaillyNotFoundException("Service package not found.");

        // Constraint: do not delete if used in any active booking
        var inUse = await context.Bookings
            .AnyAsync(b => b.ServicePackageId == request.Id && !b.IsDeleted, ct);

        if (inUse)
            throw new DetaillyBusinessRuleException("SERVICE_PACKAGE_IN_USE","Cannot delete service package because it is used in bookings.");


        // Soft delete package
        package.IsDeleted = true;
        package.ModifiedAtUtc = DateTime.UtcNow;

        // Soft delete assignments too
        var assignments = await context.ServicePackageItemAssignments
            .Where(x => x.ServicePackageId == request.Id && !x.IsDeleted)
            .ToListAsync(ct);

        foreach (var a in assignments)
        {
            a.IsDeleted = true;
            a.ModifiedAtUtc = DateTime.UtcNow;
        }

        await context.SaveChangesAsync(ct);
        await transaction.CommitAsync(ct);

        return Unit.Value;
    }
}
