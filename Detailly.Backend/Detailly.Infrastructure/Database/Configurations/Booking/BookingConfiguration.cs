
using Detailly.Domain.Entities.Booking;
using Detailly.Domain.Entities.Payment;
using Microsoft.EntityFrameworkCore;

namespace Detailly.Infrastructure.Database.Configurations.Booking;
public class BookingConfiguration : IEntityTypeConfiguration<BookingEntity>
{
    public void Configure(EntityTypeBuilder<BookingEntity> builder)
    {
        builder
            .ToTable("Bookings") // ime tabele
            .HasOne(b => b.Review)
            .WithOne(r => r.Booking)
            .HasForeignKey<ReviewEntity>(r => r.BookingId) // shared PK i FK
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(b => b.PaymentTransaction)
            .WithOne(p => p.Booking)
            .HasForeignKey<PaymentTransactionEntity>(p => p.BookingId)
            .OnDelete(DeleteBehavior.Restrict);

    }
}
