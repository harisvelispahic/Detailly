using Detailly.Domain.Entities.Booking;
using Detailly.Domain.Entities.Payment;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Detailly.Infrastructure.Database.Configurations.Payment;

public sealed class PaymentTransactionConfiguration
    : IEntityTypeConfiguration<PaymentTransactionEntity>
{
    public void Configure(EntityTypeBuilder<PaymentTransactionEntity> builder)
    {
        builder.ToTable("PaymentTransactions");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Amount)
            .IsRequired();

        builder.Property(x => x.TransactionType)
            .IsRequired();

        builder.Property(x => x.Status)
            .IsRequired();

        // BOOKING (1 : 0..1)
        builder.HasOne(x => x.Booking)
            .WithOne(x => x.PaymentTransaction)
            .HasForeignKey<PaymentTransactionEntity>(x => x.BookingId)
            .OnDelete(DeleteBehavior.Restrict);

        // WALLET (1 : many)
        builder.HasOne(x => x.Wallet)
            .WithMany(x => x.PaymentTransactions)
            .HasForeignKey(x => x.WalletId)
            .OnDelete(DeleteBehavior.Restrict);

        // ORDER (1 : many)
        builder.HasOne(x => x.Order)
            .WithMany()
            .HasForeignKey(x => x.OrderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.ProviderTransactionId)
            .IsUnique()
            .HasFilter("[ProviderTransactionId] IS NOT NULL");
    }
}
