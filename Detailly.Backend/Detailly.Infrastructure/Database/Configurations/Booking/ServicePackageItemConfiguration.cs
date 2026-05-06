using Detailly.Domain.Entities.Booking;

namespace Detailly.Infrastructure.Database.Configurations.Booking;

public sealed class ServicePackageItemConfiguration : IEntityTypeConfiguration<ServicePackageItemEntity>
{
    public void Configure(EntityTypeBuilder<ServicePackageItemEntity> builder)
    {
        builder.ToTable("ServicePackageItems");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired();

        builder.Property(x => x.Price)
            .HasPrecision(18, 2);

        builder.Property(x => x.Description)
            .IsRequired(false);

        builder.Property(x => x.DurationMinutes)
            .IsRequired();

        builder.Property(x => x.RequiredEmployees)
            .IsRequired();

        builder.Property(x => x.IsAddon)
            .IsRequired();

        builder.Property(x => x.IsActive)
            .IsRequired();

        // Other relationships left to conventions (assignments, booking items) due to missing FK certainty.
    }
}