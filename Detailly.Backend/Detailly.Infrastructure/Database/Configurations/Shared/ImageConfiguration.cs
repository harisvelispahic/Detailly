using Detailly.Domain.Entities.Shared;

namespace Detailly.Infrastructure.Database.Configurations.Shared;

public sealed class ImageConfiguration : IEntityTypeConfiguration<ImageEntity>
{
    public void Configure(EntityTypeBuilder<ImageEntity> builder)
    {
        builder.ToTable("Images");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ImageUrl)
            .IsRequired();

        builder.Property(x => x.PublicId)
            .IsRequired(false);

        builder.Property(x => x.AltText)
            .IsRequired(false);

        builder.Property(x => x.IsThumbnail)
            .IsRequired();

        builder.Property(x => x.DisplayOrder)
            .IsRequired();

        // Product relationship is owned by ProductConfiguration (HasMany side).
        // ServicePackage relationship is owned by ServicePackageConfiguration (HasMany side).
    }
}