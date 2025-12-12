
using Detailly.Application.Modules.Booking.Reviews.Commands.Create;
using Detailly.Domain.Entities.Booking;
using Detailly.Domain.Entities.Shared;

public class CreateReviewCommandHandler(IAppDbContext context, IAppCurrentUser currentUser)
    : IRequestHandler<CreateReviewCommand, int>
{
    public async Task<int> Handle(CreateReviewCommand request, CancellationToken ct)
    {
        // Get booking and verify it exists
        var booking = await context.Bookings
            .FirstOrDefaultAsync(x => x.Id == request.BookingId, ct);

        if (booking is null)
            throw new DetaillyNotFoundException("Booking not found.");

        // Authorization check: user must own the booking
        if (booking.ApplicationUserId != currentUser.ApplicationUserId)
            throw new DetaillyForbiddenException("You are not allowed to review this booking.");

        // Check if a review already exists
        var existingReview = await context.Reviews
            .FirstOrDefaultAsync(x => x.BookingId == request.BookingId, ct);

        if (existingReview is not null && !existingReview.IsDeleted)
        {
            throw new DetaillyConflictException("Review for this booking already exists.");
        }

        // Optional: only allow review if booking is completed
        // if (booking.Status != BookingStatus.Completed)
        //     throw new ValidationException("Cannot leave a review before the booking is completed.");


        // If soft-deleted review exists → restore it
        if (existingReview is not null && existingReview.IsDeleted)
        {
            existingReview.IsDeleted = false;
            existingReview.Rating = request.Rating;
            existingReview.Description = request.Description?.Trim();
            existingReview.ValueForMoney = request.ValueForMoney;

            existingReview.Images = request.Images?
                .Select(img => new ImageEntity
                {
                    ImageUrl = img.ImageUrl.Trim(),
                    AltText = img.AltText?.Trim(),
                    IsThumbnail = img.IsThumbnail,
                    DisplayOrder = img.DisplayOrder
                }).ToList() ?? new List<ImageEntity>();

            existingReview.ModifiedAtUtc = DateTime.UtcNow;

            await context.SaveChangesAsync(ct);
            return existingReview.BookingId;
        }


        // Otherwise, create a new review
        var review = new ReviewEntity
        {
            BookingId = request.BookingId,
            Rating = request.Rating,
            Description = request.Description?.Trim(),
            ValueForMoney = request.ValueForMoney,

            Images = request.Images?
                .Select(img => new ImageEntity
                {
                    ImageUrl = img.ImageUrl.Trim(),
                    AltText = img.AltText?.Trim(),
                    IsThumbnail = img.IsThumbnail,
                    DisplayOrder = img.DisplayOrder
                })
                .ToList() ?? new List<ImageEntity>(),

            CreatedAtUtc = DateTime.UtcNow
        };

        context.Reviews.Add(review);
        await context.SaveChangesAsync(ct);

        return review.BookingId;
    }
}


//using Detailly.Application.Modules.Booking.Reviews.Commands.Create;
//using Detailly.Domain.Entities.Booking;
//using Detailly.Domain.Entities.Shared;

//public class CreateReviewCommandHandler(IAppDbContext context, IAppCurrentUser currentUser)
//    : IRequestHandler<CreateReviewCommand, int>
//{
//    public async Task<int> Handle(CreateReviewCommand request, CancellationToken ct)
//    {
//        var existingReview = await context.Reviews
//            .FirstOrDefaultAsync(x => x.BookingId == request.BookingId, ct);

//        if (existingReview is not null && !existingReview.IsDeleted)
//        {
//            throw new DetaillyConflictException("Review for this booking already exists.");
//        }

//        // Optional: verify the booking exists
//        var booking = await context.Bookings
//            .FirstOrDefaultAsync(x => x.Id == request.BookingId, ct);

//        if (booking is null)
//            throw new DetaillyNotFoundException("Booking not found.");

//        // Optional: only allow review if the booking is completed
//        // if (booking.Status != BookingStatus.Completed)
//        //     throw new ValidationException("Cannot leave a review before the booking is completed.");

//        // If soft-deleted review existed → reuse it
//        if (existingReview is not null && existingReview.IsDeleted)
//        {
//            existingReview.IsDeleted = false;
//            existingReview.Rating = request.Rating;
//            existingReview.Description = request.Description?.Trim();
//            existingReview.ValueForMoney = request.ValueForMoney;

//            existingReview.Images = request.Images?
//                .Select(img => new ImageEntity
//                {
//                    ImageUrl = img.ImageUrl.Trim(),
//                    AltText = img.AltText?.Trim(),
//                    IsThumbnail = img.IsThumbnail,
//                    DisplayOrder = img.DisplayOrder
//                })
//                .ToList() ?? new List<ImageEntity>();

//            existingReview.ModifiedAtUtc = DateTime.UtcNow;

//            await context.SaveChangesAsync(ct);

//            return existingReview.BookingId;
//        }

//        // Create new review
//        var review = new ReviewEntity
//        {
//            BookingId = request.BookingId,
//            Rating = request.Rating,
//            Description = request.Description?.Trim(),
//            ValueForMoney = request.ValueForMoney,
//            Images = request.Images?.Select(img => new ImageEntity
//            {
//                ImageUrl = img.ImageUrl.Trim(),
//                AltText = img.AltText?.Trim(),
//                IsThumbnail = img.IsThumbnail,
//                DisplayOrder = img.DisplayOrder
//            }).ToList() ?? new List<ImageEntity>(),
//            CreatedAtUtc = DateTime.UtcNow
//        };

//        context.Reviews.Add(review);
//        await context.SaveChangesAsync(ct);

//        return review.BookingId;
//    }
//}
