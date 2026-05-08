namespace Detailly.Application.Modules.Booking.Reviews.Queries.ListMy;

public sealed class ListMyReviewsQueryValidator : AbstractValidator<ListMyReviewsQuery>
{
    public ListMyReviewsQueryValidator()
    {
        RuleFor(x => x.Sort)
            .Must(s => s is null || s == "newest" || s == "oldest")
            .WithMessage("Sort must be 'newest' or 'oldest'.");
    }
}
