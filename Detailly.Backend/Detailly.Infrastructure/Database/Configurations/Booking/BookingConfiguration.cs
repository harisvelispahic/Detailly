using Detailly.Domain.Entities.Booking;
using Detailly.Domain.Entities.Payment;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Detailly.Infrastructure.Database.Configurations.Booking;

public sealed class BookingConfiguration : IEntityTypeConfiguration<BookingEntity>
{
    public void Configure(EntityTypeBuilder<BookingEntity> builder)
    {
        builder.ToTable("Bookings");
        builder.HasKey(x => x.Id);

        builder.HasOne(b => b.Review)
            .WithOne(r => r.Booking)
            .HasForeignKey<ReviewEntity>(r => r.BookingId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(b => b.PaymentTransactions)
            .WithOne(p => p.Booking)
            .HasForeignKey(p => p.BookingId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Customer)
            .WithMany(u => u.Bookings)
            .HasForeignKey(x => x.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.ShopLocation)
            .WithMany() // you don't need a collection
            .HasForeignKey(x => x.ShopLocationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.ServiceAddress)
            .WithMany() // address does not need collection
            .HasForeignKey(x => x.ServiceAddressId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes for availability queries (critical)
        builder.HasIndex(x => new { x.ShopLocationId, x.StartUtc, x.EndUtc });
        builder.HasIndex(x => new { x.Status, x.ReservationExpiresAtUtc });
        builder.HasIndex(x => new { x.ServiceMode, x.StartUtc, x.EndUtc });
    }
}