using Detailly.Domain.Entities.Booking;

namespace Detailly.Infrastructure.Database.Configurations.Booking;

public sealed class ServicePackageConfiguration : IEntityTypeConfiguration<ServicePackageEntity>
{
    public void Configure(EntityTypeBuilder<ServicePackageEntity> builder)
    {
        builder.ToTable("ServicePackages");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired();

        builder.Property(x => x.Description)
            .IsRequired(false);

        builder.Property(x => x.Price)
            .HasPrecision(18, 2);

        builder.Property(x => x.BaseDurationMinutes)
            .IsRequired(false);

        builder.Property(x => x.BaseRequiredEmployees)
            .IsRequired(false);

        builder.HasMany(x => x.Images)
            .WithOne(x => x.ServicePackage)
            .HasForeignKey(x => x.ServicePackageId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}