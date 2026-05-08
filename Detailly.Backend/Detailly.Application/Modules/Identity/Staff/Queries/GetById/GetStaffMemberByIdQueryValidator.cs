namespace Detailly.Application.Modules.Identity.Staff.Queries.GetById;

public sealed class GetStaffMemberByIdQueryValidator : AbstractValidator<GetStaffMemberByIdQuery>
{
    public GetStaffMemberByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Staff member ID must be greater than zero.");
    }
}
