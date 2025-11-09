
using Detailly.Domain.Entities.Booking;
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
    }
}
