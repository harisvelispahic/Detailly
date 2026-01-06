using FluentValidation;

namespace Detailly.Application.Modules.Booking.ServicePackages.Commands.Delete;

public class DeleteServicePackageCommandValidator : AbstractValidator<DeleteServicePackageCommand>
{
    public DeleteServicePackageCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Id must be greater than zero.");
    }
}
