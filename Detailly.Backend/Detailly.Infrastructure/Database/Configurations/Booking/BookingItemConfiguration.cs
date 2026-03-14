using Detailly.Domain.Entities.Booking;

namespace Detailly.Infrastructure.Database.Configurations.Booking;

public sealed class BookingItemConfiguration : IEntityTypeConfiguration<BookingItemEntity>
{
    public void Configure(EntityTypeBuilder<BookingItemEntity> builder)
    {
        builder.ToTable("BookingItems");
        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.Booking)
            .WithMany(b => b.BookingItems)
            .HasForeignKey(x => x.BookingId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.ServicePackageItem)
            .WithMany(spi => spi.BookingItems)
            .HasForeignKey(x => x.ServicePackageItemId)
            .OnDelete(DeleteBehavior.Restrict);

        // Prevent selecting same addon twice for same booking
        builder.HasIndex(x => new { x.BookingId, x.ServicePackageItemId })
            .IsUnique();

        builder.Property(x => x.IsAddon)
            .HasDefaultValue(true);

        builder.HasIndex(x => x.ServicePackageItemId);
    }
}