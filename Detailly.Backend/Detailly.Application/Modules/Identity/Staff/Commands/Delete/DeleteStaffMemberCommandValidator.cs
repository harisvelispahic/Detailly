namespace Detailly.Application.Modules.Identity.Staff.Commands.Delete;

public sealed class DeleteStaffMemberCommandValidator : AbstractValidator<DeleteStaffMemberCommand>
{
    public DeleteStaffMemberCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Staff member ID must be greater than zero.");
    }
}
