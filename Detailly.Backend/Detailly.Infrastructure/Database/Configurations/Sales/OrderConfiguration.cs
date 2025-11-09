
using Detailly.Domain.Entities.Payment;
using Detailly.Domain.Entities.Sales;

namespace Detailly.Infrastructure.Database.Configurations.Sales;

public class OrderConfiguration : IEntityTypeConfiguration<OrderEntity>
{
    public void Configure(EntityTypeBuilder<OrderEntity> builder)
    {
        builder
            .ToTable("Orders");

        builder
            .HasOne(o => o.PaymentTransaction)
            .WithOne(pt => pt.Order)
            .HasForeignKey<PaymentTransactionEntity>(pt => pt.OrderId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
