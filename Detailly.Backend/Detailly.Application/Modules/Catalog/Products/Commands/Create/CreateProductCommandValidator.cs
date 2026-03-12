namespace Detailly.Application.Modules.Catalog.Products.Commands.Create;

public sealed class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator(IAppDbContext context)
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(ProductEntity.Constraints.NameMaxLength)
            .Must(x => !string.IsNullOrWhiteSpace(x))
            .WithMessage("Name is required.")
            .MustAsync(async (name, ct) =>
            {
                var normalized = name.Trim();
                return !await context.Products.AnyAsync(p => p.Name == normalized, ct);
            })
            .WithMessage("Name already exists.");

        RuleFor(x => x.Description)
            .NotEmpty()
            .MaximumLength(ProductEntity.Constraints.DescriptionMaxLength)
            .Must(x => !string.IsNullOrWhiteSpace(x))
            .WithMessage("Description is required.");

        RuleFor(x => x.Price)
            .GreaterThan(0);

        RuleFor(x => x.CategoryId)
            .GreaterThan(0);

        RuleFor(x => x.Inventory!)
            .SetValidator(new CreateProductCommandInventoryValidator()!)
            .When(x => x.Inventory is not null);

        RuleForEach(x => x.Images)
            .SetValidator(new CreateProductCommandImageValidator());
    }
}

public sealed class CreateProductCommandInventoryValidator : AbstractValidator<CreateProductCommandInventory>
{
    public CreateProductCommandInventoryValidator()
    {
        RuleFor(x => x.QuantityInStock)
            .GreaterThanOrEqualTo(0)
            .When(x => x.QuantityInStock.HasValue);

        RuleFor(x => x.ReorderLevel)
            .GreaterThanOrEqualTo(0)
            .When(x => x.ReorderLevel.HasValue);

        RuleFor(x => x.ReorderQuantity)
            .GreaterThanOrEqualTo(0)
            .When(x => x.ReorderQuantity.HasValue);
    }
}

public sealed class CreateProductCommandImageValidator : AbstractValidator<CreateProductCommandImage>
{
    public CreateProductCommandImageValidator()
    {
        RuleFor(x => x.ImageUrl)
            .NotEmpty();

        RuleFor(x => x.DisplayOrder)
            .GreaterThanOrEqualTo(0)
            .When(x => x.DisplayOrder.HasValue);
    }
}