namespace Detailly.Application.Modules.Shared.Address.Commands.Delete;

public sealed class DeleteAddressCommandValidator : AbstractValidator<DeleteAddressCommand>
{
    public DeleteAddressCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Address id must be greater than 0.");
    }
}