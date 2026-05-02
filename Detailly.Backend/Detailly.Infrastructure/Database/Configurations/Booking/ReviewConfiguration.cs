using Detailly.Domain.Entities.Booking;

namespace Detailly.Infrastructure.Database.Configurations.Booking;

public class ReviewConfiguration : IEntityTypeConfiguration<ReviewEntity>
{
    public void Configure(EntityTypeBuilder<ReviewEntity> builder)
    {
        builder.ToTable("Reviews");
        builder.HasKey(r => r.Id);

        builder.HasOne(r => r.Booking)
            .WithMany()
            .HasForeignKey(r => r.BookingId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.ServicePackage)
            .WithMany()
            .HasForeignKey(r => r.ServicePackageId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(r => r.Customer)
            .WithMany()
            .HasForeignKey(r => r.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(r => new { r.CustomerId, r.ServicePackageId })
            .IsUnique();
    }
}
