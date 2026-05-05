using Detailly.Domain.Entities.Booking;

namespace Detailly.Infrastructure.Database.Configurations.Booking;

public class ReactionConfiguration : IEntityTypeConfiguration<ReactionEntity>
{
    public void Configure(EntityTypeBuilder<ReactionEntity> builder)
    {
        builder.ToTable("Reactions");
        builder.HasKey(r => r.Id);

        builder.HasOne(r => r.ServicePackage)
            .WithMany()
            .HasForeignKey(r => r.ServicePackageId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(r => r.Customer)
            .WithMany()
            .HasForeignKey(r => r.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(r => new { r.CustomerId, r.ServicePackageId }).IsUnique();
        builder.HasIndex(r => r.ServicePackageId);
    }
}
