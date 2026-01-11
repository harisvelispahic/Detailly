using FluentValidation;

namespace Detailly.Application.Modules.Booking.ServicePackages.Queries.List;

public sealed class ListServicePackagesQueryValidator : AbstractValidator<ListServicePackagesQuery>
{
    public ListServicePackagesQueryValidator()
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
