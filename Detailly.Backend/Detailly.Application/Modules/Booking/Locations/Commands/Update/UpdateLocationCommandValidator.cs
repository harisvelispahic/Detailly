namespace Detailly.Application.Modules.Booking.Locations.Commands.Update;

public sealed class UpdateLocationCommandValidator : AbstractValidator<UpdateLocationCommand>
{
    public UpdateLocationCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Location ID must be greater than zero.");

        RuleFor(x => x.Name)
            .MaximumLength(200)
            .When(x => x.Name is not null)
            .WithMessage("Name cannot exceed 200 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(1000)
            .When(x => x.Description is not null)
            .WithMessage("Description cannot exceed 1000 characters.");

        RuleFor(x => x.TotalBays)
            .GreaterThan(0)
            .When(x => x.TotalBays.HasValue)
            .WithMessage("Total bays must be greater than zero.");

        When(x => x.Address is not null, () =>
        {
            RuleFor(x => x.Address!.Street)
                .MaximumLength(200)
                .When(x => x.Address!.Street is not null)
                .WithMessage("Street cannot exceed 200 characters.");

            RuleFor(x => x.Address!.City)
                .MaximumLength(100)
                .When(x => x.Address!.City is not null)
                .WithMessage("City cannot exceed 100 characters.");

            RuleFor(x => x.Address!.PostalCode)
                .MaximumLength(20)
                .When(x => x.Address!.PostalCode is not null)
                .WithMessage("Postal code cannot exceed 20 characters.");

            RuleFor(x => x.Address!.Country)
                .MaximumLength(100)
                .When(x => x.Address!.Country is not null)
                .WithMessage("Country cannot exceed 100 characters.");
        });
    }
}
