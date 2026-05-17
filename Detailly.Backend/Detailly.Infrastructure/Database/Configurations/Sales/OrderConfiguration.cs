using Detailly.Domain.Entities.Sales;

namespace Detailly.Infrastructure.Database.Configurations.Sales;

public sealed class OrderConfiguration : IEntityTypeConfiguration<OrderEntity>
{
    public void Configure(EntityTypeBuilder<OrderEntity> builder)
    {
        builder.ToTable("Orders");

        builder.HasKey(o => o.Id);

        builder.HasIndex(o => o.OrderNumber).IsUnique();
        builder.HasIndex(o => o.ApplicationUserId);

        builder.Property(o => o.TotalAmount).HasPrecision(18, 2);

        builder.Property(o => o.Notes).IsRequired(false);

        // Order -> OrderItems (1:N)
        builder.HasMany(o => o.OrderItems)
            .WithOne(oi => oi.Order)
            .HasForeignKey(oi => oi.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        // ✅ Order -> PaymentTransactions (1:N) (audit-safe)
        builder.HasMany(o => o.PaymentTransactions)
            .WithOne(pt => pt.Order)
            .HasForeignKey(pt => pt.OrderId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}