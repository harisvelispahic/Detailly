using Detailly.Application.Abstractions;

namespace Detailly.Application.Modules.Booking.ServicePackages.Commands.Image.Delete;

public class DeleteServicePackageImageCommandHandler(
    IAppDbContext context,
    ICloudinaryService cloudinaryService)
    : IRequestHandler<DeleteServicePackageImageCommand, Unit>
{
    public async Task<Unit> Handle(DeleteServicePackageImageCommand request, CancellationToken ct)
    {
        var image = await context.Images
            .FirstOrDefaultAsync(
                i => i.Id == request.ImageId && i.ServicePackageId == request.ServicePackageId, ct);

        if (image is null)
            throw new DetaillyNotFoundException("Image not found.");

        bool wasThumbnail = image.IsThumbnail;

        if (image.PublicId is not null)
            await cloudinaryService.DeleteAsync(image.PublicId, ct);

        context.Images.Remove(image);
        await context.SaveChangesAsync(ct);

        // Promote the next image to thumbnail if the deleted one was the thumbnail
        if (wasThumbnail)
        {
            var next = await context.Images
                .Where(i => i.ServicePackageId == request.ServicePackageId)
                .OrderBy(i => i.DisplayOrder)
                .FirstOrDefaultAsync(ct);

            if (next is not null)
            {
                next.IsThumbnail = true;
                await context.SaveChangesAsync(ct);
            }
        }

        return Unit.Value;
    }
}
