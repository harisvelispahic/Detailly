
using Detailly.Domain.Entities.Shared;

namespace Detailly.Application.Modules.Booking.Reviews.Commands.Update;

public class UpdateReviewCommandHandler(IAppDbContext context, IAppCurrentUser currentUser)
    : IRequestHandler<UpdateReviewCommand, Unit>
{
    public async Task<Unit> Handle(UpdateReviewCommand request, CancellationToken ct)
    {
        await using var transaction = await context.Database.BeginTransactionAsync(ct);

        // Load review + booking + images
        var review = await context.Reviews
            .Include(r => r.Images)
            .Include(r => r.Booking)
            .FirstOrDefaultAsync(x => x.BookingId == request.BookingId, ct);

        if (review is null || review.IsDeleted)
            throw new DetaillyNotFoundException("Review not found.");

        // Authorization
        if (review.Booking.ApplicationUserId != currentUser.ApplicationUserId)
            throw new DetaillyForbiddenException("You are not allowed to update this review.");

        // -------- PARTIAL UPDATE --------

        if (request.Rating.HasValue)
            review.Rating = request.Rating.Value;

        if (request.Description is not null)
            review.Description = request.Description.Trim();

        if (request.ValueForMoney.HasValue)
            review.ValueForMoney = request.ValueForMoney.Value;

        // -------- IMAGES UPDATE --------
        if (request.Images is not null)
        {
            // Soft delete existing images
            foreach (var img in review.Images)
            {
                img.IsDeleted = true;
                img.ModifiedAtUtc = DateTime.UtcNow;
            }

            // Add new images (normal creation)
            review.Images = request.Images
                .Select(i => new ImageEntity
                {
                    ImageUrl = i.ImageUrl.Trim(),
                    AltText = i.AltText?.Trim(),
                    IsThumbnail = i.IsThumbnail,
                    DisplayOrder = i.DisplayOrder
                })
                .ToList();
        }

        review.ModifiedAtUtc = DateTime.UtcNow;

        await context.SaveChangesAsync(ct);
        await transaction.CommitAsync(ct);

        return Unit.Value;
    }
}
