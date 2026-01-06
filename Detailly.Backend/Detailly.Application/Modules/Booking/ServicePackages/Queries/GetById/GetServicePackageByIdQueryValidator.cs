using FluentValidation;

namespace Detailly.Application.Modules.Booking.ServicePackages.Queries.GetById;

public class GetServicePackageByIdQueryValidator : AbstractValidator<GetServicePackageByIdQuery>
{
    public GetServicePackageByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Service package ID must be greater than zero.");
    }
}
