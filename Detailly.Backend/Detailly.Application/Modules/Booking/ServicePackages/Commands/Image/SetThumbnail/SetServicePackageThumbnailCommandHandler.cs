namespace Detailly.Application.Modules.Booking.ServicePackages.Commands.Image.SetThumbnail;

public class SetServicePackageThumbnailCommandHandler(IAppDbContext context)
    : IRequestHandler<SetServicePackageThumbnailCommand, Unit>
{
    public async Task<Unit> Handle(SetServicePackageThumbnailCommand request, CancellationToken ct)
    {
        var images = await context.Images
            .Where(i => i.ServicePackageId == request.ServicePackageId)
            .ToListAsync(ct);

        if (!images.Any(i => i.Id == request.ImageId))
            throw new DetaillyNotFoundException("Image not found.");

        foreach (var img in images)
            img.IsThumbnail = img.Id == request.ImageId;

        await context.SaveChangesAsync(ct);
        return Unit.Value;
    }
}
