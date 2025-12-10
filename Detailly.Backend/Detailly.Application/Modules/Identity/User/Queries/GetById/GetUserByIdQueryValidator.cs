
namespace Detailly.Application.Modules.Identity.User.Queries.GetById;
public class GetUserByIdQueryValidator : AbstractValidator<GetUserByIdQuery>
{
    public GetUserByIdQueryValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0).WithMessage("Id must be a positive value.");
    }
}
