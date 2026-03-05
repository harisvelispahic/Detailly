using Detailly.Domain.Entities.Shared;

namespace Detailly.Infrastructure.Database.Configurations.Shared;

public sealed class AddressConfiguration : IEntityTypeConfiguration<AddressEntity>
{
    public void Configure(EntityTypeBuilder<AddressEntity> builder)
    {
        builder.ToTable("Addresses");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Street).HasMaxLength(250);
        builder.Property(x => x.City).HasMaxLength(100);
        builder.Property(x => x.PostalCode).HasMaxLength(20);
        builder.Property(x => x.Region).HasMaxLength(100);
        builder.Property(x => x.Country).HasMaxLength(100);

        builder.Property(x => x.Latitude).HasColumnType("decimal(9,6)");
        builder.Property(x => x.Longitude).HasColumnType("decimal(9,6)");

        builder.HasIndex(x => new { x.City, x.Country });

        // ✅ User (1:N) — address book
        builder.HasOne(x => x.ApplicationUser)
            .WithMany(u => u.Addresses)
            .HasForeignKey(x => x.ApplicationUserId)
            .OnDelete(DeleteBehavior.Restrict);

        // NOTE:
        // Orders relation is already via Order.ShipToAddressId (configured in OrderConfiguration or by convention).
        // Locations relation is already via Location.AddressId (if that exists in your model).
    }
}