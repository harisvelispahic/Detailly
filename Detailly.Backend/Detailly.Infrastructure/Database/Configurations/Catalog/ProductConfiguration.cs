namespace Detailly.Infrastructure.Database.Configurations.Catalog;

public class ProductConfiguration : IEntityTypeConfiguration<ProductEntity>
{
    public void Configure(EntityTypeBuilder<ProductEntity> builder)
    {
        builder
            .ToTable("Products");

        builder
            .Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(ProductEntity.Constraints.NameMaxLength);

        builder
            .Property(x => x.Description)
            .HasMaxLength(ProductEntity.Constraints.DescriptionMaxLength);

        builder
            .Property(x => x.Price)
            .HasPrecision(18, 2);


        // Product → Inventory (1:1)
        builder.HasOne(p => p.Inventory)
            .WithOne(i => i.Product)
            .HasForeignKey<InventoryEntity>(i => i.ProductId)
            .OnDelete(DeleteBehavior.Cascade); // ✅ cascade delete

        // Product → Images (1:N)
        builder.HasMany(p => p.Images)
            .WithOne(i => i.Product)
            .HasForeignKey(i => i.ProductId)
            .OnDelete(DeleteBehavior.Cascade); // ✅ cascade delete

        builder
            .HasOne(x => x.Category)
            .WithMany(x => x.Products)
            .HasForeignKey(x => x.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);// Restrict — do not allow deleting a category if it has products
    }
}
