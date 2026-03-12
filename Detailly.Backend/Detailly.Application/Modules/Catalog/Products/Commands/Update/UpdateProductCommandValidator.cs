namespace Detailly.Application.Modules.Catalog.Products.Commands.Update;

public sealed class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductCommandValidator(IAppDbContext context)
    {
        RuleFor(x => x.Id)
            .GreaterThan(0);

        RuleFor(x => x.Name)
            .MaximumLength(ProductEntity.Constraints.NameMaxLength)
            .Must(x => x is null || !string.IsNullOrWhiteSpace(x))
            .WithMessage("Name cannot be empty.")
            .MustAsync(async (command, name, ct) =>
            {
                if (string.IsNullOrWhiteSpace(name))
                    return true;

                var normalized = name.Trim();

                return !await context.Products
                    .AnyAsync(p => p.Id != command.Id && p.Name == normalized, ct);
            })
            .WithMessage("Name already exists.");

        RuleFor(x => x.Description)
            .MaximumLength(ProductEntity.Constraints.DescriptionMaxLength)
            .Must(x => x is null || !string.IsNullOrWhiteSpace(x))
            .WithMessage("Description cannot be empty.");

        RuleFor(x => x.Price)
            .GreaterThan(0)
            .When(x => x.Price.HasValue);

        RuleFor(x => x.Inventory!)
            .SetValidator(new UpdateProductCommandInventoryValidator()!)
            .When(x => x.Inventory is not null);
    }
}

public sealed class UpdateProductCommandInventoryValidator : AbstractValidator<UpdateProductCommandInventory>
{
    public UpdateProductCommandInventoryValidator()
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