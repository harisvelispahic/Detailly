namespace Detailly.Application.Modules.Identity.Staff.Queries.List;

public sealed class ListStaffMembersQueryValidator : AbstractValidator<ListStaffMembersQuery>
{
    public ListStaffMembersQueryValidator()
    {
        RuleFor(x => x.Search)
            .Custom((value, context) =>
            {
                var trimmed = value?.Trim();
                if (!string.IsNullOrEmpty(trimmed) && trimmed.Length > 50)
                    context.AddFailure("Search must be at most 50 characters long.");
            });
    }
}
