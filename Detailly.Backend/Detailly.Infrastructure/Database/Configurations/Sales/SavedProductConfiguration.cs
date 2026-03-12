using Detailly.Domain.Entities.Sales;

namespace Detailly.Infrastructure.Database.Configurations.Sales;

public sealed class SavedProductConfiguration : IEntityTypeConfiguration<SavedProductEntity>
{
    public void Configure(EntityTypeBuilder<SavedProductEntity> builder)
    {
        builder.ToTable("SavedProducts");

        builder.HasKey(x => x.Id);

        builder.HasIndex(x => new { x.ApplicationUserId, x.ProductId })
            .IsUnique();

        builder.HasOne(x => x.ApplicationUser)
            .WithMany(u => u.SavedProducts)
            .HasForeignKey(x => x.ApplicationUserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Product)
            .WithMany(p => p.SavedProducts)
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}