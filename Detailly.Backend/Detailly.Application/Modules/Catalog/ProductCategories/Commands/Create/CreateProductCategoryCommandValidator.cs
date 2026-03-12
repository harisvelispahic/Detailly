namespace Detailly.Application.Modules.Catalog.ProductCategories.Commands.Create;

public sealed class CreateProductCategoryCommandValidator
    : AbstractValidator<CreateProductCategoryCommand>
{
    public CreateProductCategoryCommandValidator(IAppDbContext context)
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage("Name is required.")
            .MaximumLength(ProductCategoryEntity.Constraints.NameMaxLength)
            .WithMessage($"Name can be at most {ProductCategoryEntity.Constraints.NameMaxLength} characters long.")
            .MustAsync(async (name, ct) =>
            {
                var normalized = name.Trim();

                return !await context.ProductCategories
                    .AnyAsync(x => x.Name == normalized, ct);
            })
            .WithMessage("Name already exists.");

        RuleFor(x => x.Description)
            .MaximumLength(ProductCategoryEntity.Constraints.DescriptionMaxLength)
            .When(x => x.Description is not null);
    }
}