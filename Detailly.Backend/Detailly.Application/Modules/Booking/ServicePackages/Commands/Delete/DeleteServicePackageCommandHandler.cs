using Detailly.Application.Abstractions;

namespace Detailly.Application.Modules.Booking.ServicePackages.Commands.Delete;

public class DeleteServicePackageCommandHandler(IAppDbContext context, ICloudinaryService cloudinaryService)
    : IRequestHandler<DeleteServicePackageCommand, Unit>
{
    public async Task<Unit> Handle(DeleteServicePackageCommand request, CancellationToken ct)
    {
        await using var transaction = await context.Database.BeginTransactionAsync(ct);

        var package = await context.ServicePackages
            .FirstOrDefaultAsync(x => x.Id == request.Id, ct);

        if (package is null || package.IsDeleted)
            throw new DetaillyNotFoundException("Service package not found.");

        var inUse = await context.Bookings
            .AnyAsync(b => b.ServicePackageId == request.Id, ct);

        if (inUse)
            throw new DetaillyBusinessRuleException("SERVICE_PACKAGE_IN_USE", "Cannot delete service package because it is used in bookings.");

        // Hard-delete images: remove from Cloudinary then from DB
        var images = await context.Images
            .Where(i => i.ServicePackageId == request.Id)
            .ToListAsync(ct);

        foreach (var image in images.Where(i => !string.IsNullOrEmpty(i.PublicId)))
        {
            try { await cloudinaryService.DeleteAsync(image.PublicId!, ct); }
            catch { /* continue even if Cloudinary deletion fails for one asset */ }
        }

        context.Images.RemoveRange(images);

        var now = DateTime.UtcNow;

        var assignments = await context.ServicePackageItemAssignments
            .Where(x => x.ServicePackageId == request.Id)
            .ToListAsync(ct);

        foreach (var a in assignments)
        {
            a.IsDeleted = true;
            a.ModifiedAtUtc = now;
        }

        var reviews = await context.Reviews
            .Where(r => r.ServicePackageId == request.Id)
            .ToListAsync(ct);

        foreach (var r in reviews)
        {
            r.IsDeleted = true;
            r.ModifiedAtUtc = now;
        }

        var reactions = await context.Reactions
            .Where(r => r.ServicePackageId == request.Id)
            .ToListAsync(ct);

        foreach (var r in reactions)
        {
            r.IsDeleted = true;
            r.ModifiedAtUtc = now;
        }

        package.IsDeleted = true;
        package.ModifiedAtUtc = now;

        await context.SaveChangesAsync(ct);
        await transaction.CommitAsync(ct);

        return Unit.Value;
    }
}
