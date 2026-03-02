using Detailly.Domain.Entities.Sales;

namespace Detailly.Infrastructure.Database.Configurations.Sales;

public sealed class CartConfiguration : IEntityTypeConfiguration<CartEntity>
{
    public void Configure(EntityTypeBuilder<CartEntity> builder)
    {
        builder.ToTable("Carts");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.IsEmpty)
            .IsRequired();

        builder.Property(x => x.TotalAmount)
            .HasPrecision(18, 2);

        builder.Property(x => x.Notes)
            .IsRequired(false);

        builder.Property(x => x.Status)
            .IsRequired();

        builder.HasOne(x => x.ApplicationUser)
            .WithMany() // don't assume ApplicationUserEntity has Carts nav
            .HasForeignKey(x => x.ApplicationUserId);

        // Let CartItems relationship be handled by CartItemEntity config (safe either way).
    }
}