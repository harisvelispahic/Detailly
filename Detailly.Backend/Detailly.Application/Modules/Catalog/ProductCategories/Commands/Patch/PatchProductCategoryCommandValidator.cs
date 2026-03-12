namespace Detailly.Application.Modules.Catalog.ProductCategories.Commands.Patch;

public sealed class PatchProductCategoryCommandValidator
    : AbstractValidator<PatchProductCategoryCommand>
{
    public PatchProductCategoryCommandValidator(IAppDbContext context)
    {
        RuleFor(x => x.Id)
            .GreaterThan(0);

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name cannot be empty.")
            .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage("Name cannot be empty.")
            .MaximumLength(ProductCategoryEntity.Constraints.NameMaxLength)
            .WithMessage($"Name can be at most {ProductCategoryEntity.Constraints.NameMaxLength} characters long.")
            .MustAsync(async (command, name, ct) =>
            {
                if (string.IsNullOrWhiteSpace(name))
                    return true;

                var normalized = name.Trim();

                return !await context.ProductCategories
                    .AnyAsync(x => x.Id != command.Id && x.Name == normalized, ct);
            })
            .WithMessage("Name already exists.")
            .When(x => x.Name is not null);

        RuleFor(x => x.Description)
            .MaximumLength(ProductCategoryEntity.Constraints.DescriptionMaxLength)
            .When(x => x.Description is not null);
    }
}