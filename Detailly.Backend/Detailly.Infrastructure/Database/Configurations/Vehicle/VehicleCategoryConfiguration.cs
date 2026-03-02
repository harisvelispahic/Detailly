using Detailly.Domain.Entities.Vehicle;

namespace Detailly.Infrastructure.Database.Configurations.Vehicle;

public sealed class VehicleCategoryConfiguration : IEntityTypeConfiguration<VehicleCategoryEntity>
{
    public void Configure(EntityTypeBuilder<VehicleCategoryEntity> builder)
    {
        builder.ToTable("VehicleCategories");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired();

        builder.Property(x => x.Description)
            .IsRequired(false);

        builder.Property(x => x.BasePriceMultiplier)
            .HasPrecision(10, 4);

        // Vehicles relationship left to conventions (we don't see VehicleEntity FK here).
    }
}