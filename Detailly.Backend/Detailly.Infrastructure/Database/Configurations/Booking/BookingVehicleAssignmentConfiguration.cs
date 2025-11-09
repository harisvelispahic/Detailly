
using Detailly.Domain.Entities.Booking;

namespace Detailly.Infrastructure.Database.Configurations.Booking;
public class BookingVehicleAssignmentConfiguration: IEntityTypeConfiguration<BookingVehicleAssignmentEntity>
{
    public void Configure(EntityTypeBuilder<BookingVehicleAssignmentEntity> builder)
    {
        builder
            .ToTable("BookingVehicleAssignments");

        builder
            .HasOne(bva => bva.Vehicle)
            .WithMany(v => (IEnumerable<BookingVehicleAssignmentEntity>)v.BookingVehicleAssignments)
            .HasForeignKey(bva => bva.VehicleId)
            .OnDelete(DeleteBehavior.Restrict); // or DeleteBehavior.NoAction

        builder
            .HasOne(bva => bva.Booking)
            .WithMany(b => (IEnumerable<BookingVehicleAssignmentEntity>)b.BookingVehicleAssignments)
            .HasForeignKey(bva => bva.BookingId)
            .OnDelete(DeleteBehavior.Restrict); // also optional
    }
}
