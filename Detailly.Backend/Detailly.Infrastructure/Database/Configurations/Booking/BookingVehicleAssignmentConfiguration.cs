using Detailly.Domain.Entities.Booking;

namespace Detailly.Infrastructure.Database.Configurations.Booking;

public sealed class BookingVehicleAssignmentConfiguration : IEntityTypeConfiguration<BookingVehicleAssignmentEntity>
{
    public void Configure(EntityTypeBuilder<BookingVehicleAssignmentEntity> builder)
    {
        builder.ToTable("BookingVehicleAssignments");
        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.Booking)
            .WithMany(b => b.BookingVehicleAssignments)
            .HasForeignKey(x => x.BookingId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Vehicle)
            .WithMany(v => v.BookingVehicleAssignments)
            .HasForeignKey(x => x.VehicleId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.BookingId, x.VehicleId })
            .IsUnique();

        builder.HasIndex(x => x.VehicleId);
    }
}