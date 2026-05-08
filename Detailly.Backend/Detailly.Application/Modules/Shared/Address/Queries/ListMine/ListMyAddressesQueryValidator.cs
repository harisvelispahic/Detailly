namespace Detailly.Application.Modules.Shared.Address.Queries.ListMine;

public sealed class ListMyAddressesQueryValidator : AbstractValidator<ListMyAddressesQuery>
{
    public ListMyAddressesQueryValidator()
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
