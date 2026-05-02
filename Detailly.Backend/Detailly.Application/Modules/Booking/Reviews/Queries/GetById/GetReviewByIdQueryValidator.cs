namespace Detailly.Application.Modules.Booking.Reviews.Queries.GetById;

public class GetReviewByIdQueryValidator : AbstractValidator<GetReviewByIdQuery>
{
    public GetReviewByIdQueryValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
    }
}
