using Detailly.Domain.Entities.Booking;

namespace Detailly.Infrastructure.Database.Configurations.Booking;

public sealed class LocationOpeningHoursConfiguration : IEntityTypeConfiguration<LocationOpeningHoursEntity>
{
    public void Configure(EntityTypeBuilder<LocationOpeningHoursEntity> builder)
    {
        builder.ToTable("LocationOpeningHours");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.DayOfWeek).IsRequired();
        builder.Property(x => x.IsClosed).IsRequired();

        builder.HasOne(x => x.ShopLocation)
            .WithMany()
            .HasForeignKey(x => x.ShopLocationId)
            .OnDelete(DeleteBehavior.Restrict);

        // One row per location per day
        builder.HasIndex(x => new { x.ShopLocationId, x.DayOfWeek })
            .IsUnique();

        builder.HasIndex(x => x.ShopLocationId);
    }
}