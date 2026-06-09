namespace Detailly.Application.Modules.Booking.Reviews.Commands.Delete;

public class DeleteReviewCommandHandler(IAppDbContext context, IAppAuthorizationService authService)
    : IRequestHandler<DeleteReviewCommand, Unit>
{
    public async Task<Unit> Handle(DeleteReviewCommand request, CancellationToken ct)
    {
        var review = await context.Reviews
            .Include(r => r.Booking)
            .FirstOrDefaultAsync(x => x.Id == request.Id, ct);

        if (review is null || review.IsDeleted)
            throw new DetaillyNotFoundException("Review not found.");

        authService.EnsureOwnerOrAdmin(review.CustomerId, "review");

        review.IsDeleted = true;
        review.ModifiedAtUtc = DateTime.UtcNow;

        await context.SaveChangesAsync(ct);
        return Unit.Value;
    }
}
