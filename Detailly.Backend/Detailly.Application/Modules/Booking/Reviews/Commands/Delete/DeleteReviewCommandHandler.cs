namespace Detailly.Application.Modules.Booking.Reviews.Commands.Delete;

public class DeleteReviewCommandHandler(IAppDbContext context, IAppCurrentUser currentUser)
    : IRequestHandler<DeleteReviewCommand, Unit>
{
    public async Task<Unit> Handle(DeleteReviewCommand request, CancellationToken ct)
    {
        var review = await context.Reviews
            .Include(r => r.Booking)
            .FirstOrDefaultAsync(x => x.Id == request.Id, ct);

        if (review is null || review.IsDeleted)
            throw new DetaillyNotFoundException("Review not found.");

        if (review.CustomerId != currentUser.ApplicationUserId && !currentUser.IsAdmin)
            throw new DetaillyForbiddenException("You are not allowed to delete this review.");

        review.IsDeleted = true;
        review.ModifiedAtUtc = DateTime.UtcNow;

        await context.SaveChangesAsync(ct);
        return Unit.Value;
    }
}
