using Detailly.Domain.Entities.Booking;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Detailly.Infrastructure.Database.Configurations.Booking;

public sealed class BookingEmployeeAssignmentConfiguration : IEntityTypeConfiguration<BookingEmployeeAssignmentEntity>
{
    public void Configure(EntityTypeBuilder<BookingEmployeeAssignmentEntity> builder)
    {
        builder.ToTable("BookingEmployeeAssignments");
        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.Booking)
            .WithMany(b => b.EmployeeAssignments)
            .HasForeignKey(x => x.BookingId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Employee)
            .WithMany() // no collection needed on ApplicationUser
            .HasForeignKey(x => x.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.BookingId, x.EmployeeId }).IsUnique();
        builder.HasIndex(x => x.EmployeeId);
    }
}