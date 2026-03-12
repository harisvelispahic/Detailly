namespace Detailly.Infrastructure.Database.Configurations.Catalog;

public sealed class InventoryConfiguration : IEntityTypeConfiguration<InventoryEntity>
{
    public void Configure(EntityTypeBuilder<InventoryEntity> builder)
    {
        builder.ToTable("Inventories");

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
    }
}