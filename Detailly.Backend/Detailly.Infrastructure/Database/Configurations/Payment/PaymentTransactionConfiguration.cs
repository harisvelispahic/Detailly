using Detailly.Domain.Entities.Booking;
using Detailly.Domain.Entities.Payment;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Detailly.Infrastructure.Database.Configurations.Payment;

public sealed class PaymentTransactionConfiguration
    : IEntityTypeConfiguration<PaymentTransactionEntity>
{
    public void Configure(EntityTypeBuilder<PaymentTransactionEntity> b)
    {
        b.ToTable("PaymentTransactions");

        b.HasKey(x => x.Id);

        b.Property(x => x.Amount)
            .IsRequired();

        b.Property(x => x.TransactionType)
            .IsRequired();

        b.Property(x => x.Status)
            .IsRequired();

        // BOOKING (1 : 0..1)
        b.HasOne(x => x.Booking)
            .WithOne(x => x.PaymentTransaction)
            .HasForeignKey<PaymentTransactionEntity>(x => x.BookingId)
            .OnDelete(DeleteBehavior.Restrict);

        // WALLET (1 : many)
        b.HasOne(x => x.Wallet)
            .WithMany(x => x.PaymentTransactions)
            .HasForeignKey(x => x.WalletId)
            .OnDelete(DeleteBehavior.Restrict);

        // ORDER (1 : many)
        b.HasOne(x => x.Order)
            .WithMany()
            .HasForeignKey(x => x.OrderId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
