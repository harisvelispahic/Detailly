using Detailly.Domain.Common;

namespace Detailly.Domain.Entities.Catalog;

public class ProductCategoryEntity : BaseEntity
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public bool IsEnabled { get; set; }

    // Foreign keys
    public IReadOnlyCollection<ProductEntity> Products { get; private set; } = new List<ProductEntity>();

    /// <summary>
    /// Single source of truth for technical/business constraints.
    /// Used in validators and EF configuration.
    /// </summary>
    public static class Constraints
    {
        public const int NameMaxLength = 100;
        public const int DescriptionMaxLength = 1000;
    }
}
