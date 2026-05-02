namespace Detailly.Application.Modules.Booking.Reviews.Queries.List;

public sealed class ListReviewsQueryValidator : AbstractValidator<ListReviewsQuery>
{
    public ListReviewsQueryValidator()
    {
        RuleFor(x => x.Sort)
            .Must(s => s == null || s == "newest" || s == "oldest")
            .WithMessage("Sort must be 'newest' or 'oldest'.");
    }
}
