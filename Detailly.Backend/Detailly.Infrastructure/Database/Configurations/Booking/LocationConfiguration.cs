using Detailly.Domain.Entities.Booking;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Detailly.Infrastructure.Database.Configurations.Booking;

public sealed class LocationConfiguration : IEntityTypeConfiguration<LocationEntity>
{
    public void Configure(EntityTypeBuilder<LocationEntity> builder)
    {
        builder.ToTable("Locations");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.TotalBays)
            .IsRequired();

        builder.HasOne(x => x.Address)
            .WithMany(a => a.Locations)
            .HasForeignKey(x => x.AddressId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.Name);
    }
}