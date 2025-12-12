
namespace Detailly.Application.Modules.Booking.Reviews.Commands.Delete;

public class DeleteReviewCommandHandler(IAppDbContext context, IAppCurrentUser currentUser)
    : IRequestHandler<DeleteReviewCommand, Unit>
{
    public async Task<Unit> Handle(DeleteReviewCommand request, CancellationToken ct)
    {
        await using var transaction = await context.Database.BeginTransactionAsync(ct);

        // Load review with booking + images
        var review = await context.Reviews
            .Include(r => r.Images)
            .Include(r => r.Booking)
            .FirstOrDefaultAsync(x => x.BookingId == request.BookingId, ct);

        if (review is null || review.IsDeleted)
            throw new DetaillyNotFoundException("Review not found.");

        // Authorization: check user owns the booking
        if (review.Booking.ApplicationUserId != currentUser.ApplicationUserId)
            throw new DetaillyForbiddenException("You are not allowed to delete this review.");

        // Soft delete
        review.IsDeleted = true;
        review.ModifiedAtUtc = DateTime.UtcNow;

        foreach (var img in review.Images)
        {
            img.IsDeleted = true;
            img.ModifiedAtUtc = DateTime.UtcNow;
        }

        await context.SaveChangesAsync(ct);
        await transaction.CommitAsync(ct);

        return Unit.Value;
    }
}


//public class DeleteReviewCommandHandler(IAppDbContext context)
//    : IRequestHandler<DeleteReviewCommand, Unit>
//{
//    public async Task<Unit> Handle(DeleteReviewCommand request, CancellationToken ct)
//    {
//        // Start an EF transaction
//        await using var transaction = await context.Database.BeginTransactionAsync(ct);

//        // Load review with images
//        var review = await context.Reviews
//            .Include(r => r.Images)
//            .FirstOrDefaultAsync(x => x.BookingId == request.BookingId, ct);

//        if (review is null)
//            throw new DetaillyNotFoundException("Review was not found.");

//        if (review.IsDeleted)
//            throw new DetaillyNotFoundException("Review does not exist.");

//        // Soft delete review
//        review.IsDeleted = true;
//        review.ModifiedAtUtc = DateTime.UtcNow;

//        // Soft delete related images
//        foreach (var image in review.Images)
//        {
//            image.IsDeleted = true;
//            image.ModifiedAtUtc = DateTime.UtcNow;
//        }

//        // Save everything
//        await context.SaveChangesAsync(ct);
//        await transaction.CommitAsync(ct);

//        return Unit.Value;
//    }
//}
