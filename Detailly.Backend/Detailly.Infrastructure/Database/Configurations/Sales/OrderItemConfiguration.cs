using Detailly.Domain.Entities.Sales;

namespace Detailly.Infrastructure.Database.Configurations.Sales;

public sealed class OrderItemConfiguration : IEntityTypeConfiguration<OrderItemEntity>
{
    public void Configure(EntityTypeBuilder<OrderItemEntity> builder)
    {
        builder.ToTable("OrderItems");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.UnitPrice)
            .HasPrecision(18, 2);

        builder.Property(x => x.Quantity)
            .IsRequired();

        builder.Property(x => x.LineSubtotal)
            .HasPrecision(18, 2);

        builder.Property(x => x.DiscountPercentage)
            .HasPrecision(5, 4);

        builder.Property(x => x.LineTotal)
            .HasPrecision(18, 2);

        // FKs are non-nullable => required relationships by convention
        builder.HasOne(x => x.Order)
            .WithMany() // don't assume OrderEntity.OrderItems exists
            .HasForeignKey(x => x.OrderId);

        builder.HasOne(x => x.Product)
            .WithMany() // don't assume ProductEntity.OrderItems exists
            .HasForeignKey(x => x.ProductId);
    }
}