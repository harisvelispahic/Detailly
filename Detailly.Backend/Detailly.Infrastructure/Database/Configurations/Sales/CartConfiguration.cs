using Detailly.Domain.Entities.Sales;

namespace Detailly.Infrastructure.Database.Configurations.Sales;

public sealed class CartConfiguration : IEntityTypeConfiguration<CartEntity>
{
    public void Configure(EntityTypeBuilder<CartEntity> builder)
    {
        builder.ToTable("Carts");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Status)
            .IsRequired();

        builder.Property(x => x.TotalAmount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(x => x.IsEmpty)
            .IsRequired();

        builder.Property(x => x.Notes)
            .IsRequired(false);

        // One cart per user (1:1)
        builder.HasIndex(x => x.ApplicationUserId)
            .IsUnique();

        builder.HasOne(x => x.ApplicationUser)
            .WithOne(u => u.Cart)
            .HasForeignKey<CartEntity>(x => x.ApplicationUserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Cart -> CartItems (1:N)
        builder.HasMany(x => x.CartItems)
            .WithOne(ci => ci.Cart)
            .HasForeignKey(ci => ci.CartId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}