namespace Detailly.Infrastructure.Database.Configurations.Catalog;

public sealed class InventoryConfiguration : IEntityTypeConfiguration<InventoryEntity>
{
    public void Configure(EntityTypeBuilder<InventoryEntity> builder)
    {
        builder.ToTable("Inventories");

        // PK is ProductId (shared PK) - matches your [Key, ForeignKey(nameof(Product))]
        builder.HasKey(x => x.ProductId);

        builder.Property(x => x.QuantityInStock)
            .IsRequired();

        builder.Property(x => x.ReorderLevel)
            .IsRequired();

        builder.Property(x => x.ReorderQuantity)
            .IsRequired();

        builder.Property(x => x.IsDeleted)
            .IsRequired();

        builder.Property(x => x.CreatedAtUtc)
            .IsRequired();

        builder.Property(x => x.ModifiedAtUtc)
            .IsRequired(false);

        // Relationship: Inventory -> Product (required, because your nav is non-null and FK is non-nullable)
        // We avoid specifying the inverse navigation name because we don't know if ProductEntity has it.
        builder.HasOne(x => x.Product)
            .WithOne()
            .HasForeignKey<InventoryEntity>(x => x.ProductId);
    }
}