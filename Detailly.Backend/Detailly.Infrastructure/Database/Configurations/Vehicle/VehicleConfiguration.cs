using Detailly.Domain.Entities.Vehicle;

namespace Detailly.Infrastructure.Database.Configurations.Vehicle;

public sealed class VehicleConfiguration : IEntityTypeConfiguration<VehicleEntity>
{
    public void Configure(EntityTypeBuilder<VehicleEntity> builder)
    {
        builder.ToTable("Vehicles");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Brand).IsRequired();
        builder.Property(x => x.Model).IsRequired();
        builder.Property(x => x.LicencePlate).IsRequired();
        builder.Property(x => x.YearOfManufacture).IsRequired();

        builder.HasOne(x => x.ApplicationUser)
            .WithMany(u => u.Vehicles)
            .HasForeignKey(x => x.ApplicationUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.VehicleCategory)
            .WithMany(c => c.Vehicles)
            .HasForeignKey(x => x.VehicleCategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        // Common practical constraint: a user shouldn’t have two vehicles with the same plate.
        builder.HasIndex(x => new { x.ApplicationUserId, x.LicencePlate })
            .IsUnique();

        builder.HasIndex(x => x.VehicleCategoryId);
    }
}