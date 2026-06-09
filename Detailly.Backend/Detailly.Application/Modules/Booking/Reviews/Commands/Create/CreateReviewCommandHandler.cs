using Detailly.Application.Modules.Booking.Reviews.Commands.Create;
using Detailly.Domain.Common.Enums;
using Detailly.Domain.Entities.Booking;

public class CreateReviewCommandHandler(IAppDbContext context, IAppAuthorizationService authService)
    : IRequestHandler<CreateReviewCommand, int>
{
    public async Task<int> Handle(CreateReviewCommand request, CancellationToken ct)
    {
        var userId = authService.RequireUserId();

        var settings = await context.SystemSettings.AsNoTracking().FirstOrDefaultAsync(ct);
        var reviewWindowDays = settings?.ReviewWindowDays ?? 7;

        var booking = await context.Bookings
            .FirstOrDefaultAsync(x => x.Id == request.BookingId, ct);

        if (booking is null)
            throw new DetaillyNotFoundException("Booking not found.");

        if (booking.CustomerId != userId)
            throw new DetaillyForbiddenException("You are not allowed to review this booking.");

        if (booking.Status != BookingStatus.Completed)
            throw new DetaillyBusinessRuleException(
                "BOOKING_NOT_COMPLETED",
                "Only completed bookings can be reviewed.");

        if ((DateTime.UtcNow - booking.EndUtc).TotalDays > reviewWindowDays)
            throw new DetaillyBusinessRuleException(
                "REVIEW_WINDOW_EXPIRED",
                $"The {reviewWindowDays}-day review window for this booking has expired.");

        var existing = await context.Reviews
            .FirstOrDefaultAsync(
                r => r.CustomerId == userId && r.ServicePackageId == booking.ServicePackageId,
                ct);

        if (existing is not null)
        {
            existing.BookingId = request.BookingId;
            existing.Rating = request.Rating;
            existing.Description = request.Description?.Trim();
            existing.IsDeleted = false;
            existing.ModifiedAtUtc = DateTime.UtcNow;

            await context.SaveChangesAsync(ct);
            return existing.Id;
        }

        var review = new ReviewEntity
        {
            BookingId = request.BookingId,
            ServicePackageId = booking.ServicePackageId,
            CustomerId = userId,
            Rating = request.Rating,
            Description = request.Description?.Trim(),
            CreatedAtUtc = DateTime.UtcNow,
        };

        context.Reviews.Add(review);
        await context.SaveChangesAsync(ct);
        return review.Id;
    }
}
