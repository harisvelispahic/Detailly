using Detailly.Application.Modules.Identity.Staff.Commands.Update;

public sealed class UpdateStaffMemberCommandValidator : AbstractValidator<UpdateStaffMemberCommand>
{
    public UpdateStaffMemberCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0).WithMessage("Id is required.");

        RuleFor(x => x.FirstName)
            .MaximumLength(100).When(x => x.FirstName != null)
            .WithMessage("First name can be at most 100 characters long.");

        RuleFor(x => x.LastName)
            .MaximumLength(100).When(x => x.LastName != null)
            .WithMessage("Last name can be at most 100 characters long.");

        RuleFor(x => x.Username)
            .MaximumLength(50).When(x => x.Username != null)
            .WithMessage("Username can be at most 50 characters long.");

        RuleFor(x => x.Email)
            .EmailAddress().When(x => x.Email != null).WithMessage("Email is not in a valid format.")
            .MaximumLength(200).When(x => x.Email != null)
            .WithMessage("Email can be at most 200 characters long.");

        RuleFor(x => x.Phone)
            .MaximumLength(30).When(x => !string.IsNullOrEmpty(x.Phone))
            .WithMessage("Phone can be at most 30 characters long.");
    }
}
