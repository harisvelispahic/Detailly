using Detailly.Domain.Entities.Sales;

namespace Detailly.Infrastructure.Database.Configurations.Sales;

public sealed class CartItemConfiguration : IEntityTypeConfiguration<CartItemEntity>
{
    public void Configure(EntityTypeBuilder<CartItemEntity> builder)
    {
        builder.ToTable("CartItems");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.UnitPrice)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(x => x.Quantity)
            .IsRequired();

        builder.Property(x => x.LineTotal)
            .HasPrecision(18, 2)
            .IsRequired();

        // Prevent duplicate product rows inside one cart
        builder.HasIndex(x => new { x.CartId, x.ProductId })
            .IsUnique();

        builder.HasOne(x => x.Cart)
            .WithMany(c => c.CartItems)
            .HasForeignKey(x => x.CartId)
            .OnDelete(DeleteBehavior.Cascade);

        // Restrict deleting products that are referenced by cart items
        // (Usually you disable products instead of deleting them.)
        builder.HasOne(x => x.Product)
            .WithMany(p => p.CartItems)
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}