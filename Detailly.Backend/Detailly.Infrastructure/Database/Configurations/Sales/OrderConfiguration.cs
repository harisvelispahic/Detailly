using Detailly.Domain.Entities.Payment;
using Detailly.Domain.Entities.Sales;

namespace Detailly.Infrastructure.Database.Configurations.Sales;

public sealed class OrderConfiguration : IEntityTypeConfiguration<OrderEntity>
{
    public void Configure(EntityTypeBuilder<OrderEntity> builder)
    {
        builder.ToTable("Orders");

        builder.HasKey(o => o.Id);

        builder.HasIndex(o => o.OrderNumber).IsUnique();

        builder.Property(o => o.TotalAmount).HasPrecision(18, 2);

        builder.Property(o => o.Notes).IsRequired(false);

        // Order -> OrderItems (1:N)
        builder.HasMany(o => o.OrderItems)
            .WithOne(oi => oi.Order)
            .HasForeignKey(oi => oi.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        // Order -> PaymentTransaction (1:1 optional)
        builder.HasOne(o => o.PaymentTransaction)
            .WithOne(pt => pt.Order)
            .HasForeignKey<PaymentTransactionEntity>(pt => pt.OrderId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);
    }
}