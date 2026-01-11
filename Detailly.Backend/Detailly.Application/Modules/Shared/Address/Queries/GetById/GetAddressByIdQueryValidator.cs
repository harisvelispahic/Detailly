using FluentValidation;

namespace Detailly.Application.Modules.Shared.Address.Queries.GetById;

public class GetAddressByIdQueryValidator : AbstractValidator<GetAddressByIdQuery>
{
    public GetAddressByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Id must be a positive value.");
    }
}
