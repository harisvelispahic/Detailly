namespace Detailly.Application.Modules.Booking.Reactions.Commands.Upsert;

public sealed class UpsertReactionCommandValidator : AbstractValidator<UpsertReactionCommand>
{
    public UpsertReactionCommandValidator()
    {
        RuleFor(x => x.ServicePackageId)
            .GreaterThan(0)
            .WithMessage("Service package ID must be greater than zero.");
    }
}
