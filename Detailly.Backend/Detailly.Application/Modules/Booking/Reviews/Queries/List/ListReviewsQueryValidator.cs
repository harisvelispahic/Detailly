
namespace Detailly.Application.Modules.Booking.Reviews.Queries.List;
public sealed class ListReviewsQueryValidator : AbstractValidator<ListReviewsQuery>
{
    public ListReviewsQueryValidator()
    {
        RuleFor(x => x.Search)
            .Custom((value, context) =>
            {
                var trimmed = value?.Trim();
                if (!string.IsNullOrEmpty(trimmed) && trimmed.Length > 50)
                {
                    context.AddFailure("Search must be at most 50 characters long.");
                }
            });
    }
}