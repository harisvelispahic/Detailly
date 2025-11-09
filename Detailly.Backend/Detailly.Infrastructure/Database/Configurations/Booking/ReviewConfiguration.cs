
using Detailly.Domain.Entities.Booking;
using Microsoft.EntityFrameworkCore;

namespace Detailly.Infrastructure.Database.Configurations.Booking;
public class ReviewConfiguration : IEntityTypeConfiguration<ReviewEntity>
{
    public void Configure(EntityTypeBuilder<ReviewEntity> builder)
    {
        builder
            .ToTable("Reviews")
            .HasKey(r => r.BookingId); // shared PK
    }
}