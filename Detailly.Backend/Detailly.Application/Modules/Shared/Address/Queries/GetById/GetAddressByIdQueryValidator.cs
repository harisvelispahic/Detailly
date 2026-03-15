namespace Detailly.Application.Modules.Shared.Address.Queries.GetById;

public sealed class GetAddressByIdQueryValidator : AbstractValidator<GetAddressByIdQuery>
{
    public GetAddressByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Address id must be greater than 0.");
    }
}