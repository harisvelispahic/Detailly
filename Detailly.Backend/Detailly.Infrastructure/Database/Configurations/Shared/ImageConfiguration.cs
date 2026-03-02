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

        builder.Property(x => x.AltText)
            .IsRequired(false);

        builder.Property(x => x.IsThumbnail)
            .IsRequired();

        builder.Property(x => x.DisplayOrder)
            .IsRequired();

        // Optional relationships (all nullable FKs)
        builder.HasOne(x => x.Product)
            .WithMany()
            .HasForeignKey(x => x.ProductId);

        builder.HasOne(x => x.Review)
            .WithMany()
            .HasForeignKey(x => x.ReviewId);

        builder.HasOne(x => x.ApplicationUser)
            .WithMany()
            .HasForeignKey(x => x.ApplicationUserId);

        builder.HasOne(x => x.ServicePackageItem)
            .WithMany(i => i.Images)
            .HasForeignKey(x => x.ServicePackageItemId);
    }
}