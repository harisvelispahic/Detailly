namespace Detailly.Application.Modules.Booking.ServicePackageItems.Commands.Delete;

public sealed class DeleteServicePackageItemCommandValidator : AbstractValidator<DeleteServicePackageItemCommand>
{
    public DeleteServicePackageItemCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
    }
}
