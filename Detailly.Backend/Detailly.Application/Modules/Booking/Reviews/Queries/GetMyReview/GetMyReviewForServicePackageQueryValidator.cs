namespace Detailly.Application.Modules.Booking.Reviews.Queries.GetMyReview;

public sealed class GetMyReviewForServicePackageQueryValidator : AbstractValidator<GetMyReviewForServicePackageQuery>
{
    public GetMyReviewForServicePackageQueryValidator()
    {
        RuleFor(x => x.ServicePackageId)
            .GreaterThan(0)
            .WithMessage("Service package ID must be greater than zero.");
    }
}
