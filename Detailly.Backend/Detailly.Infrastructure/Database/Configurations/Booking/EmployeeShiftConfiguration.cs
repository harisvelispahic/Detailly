using Detailly.Domain.Entities.Booking;

namespace Detailly.Infrastructure.Database.Configurations.Booking;

public sealed class EmployeeShiftConfiguration : IEntityTypeConfiguration<EmployeeShiftEntity>
{
    public void Configure(EntityTypeBuilder<EmployeeShiftEntity> builder)
    {
        builder.ToTable("EmployeeShifts");
        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.Employee)
            .WithMany()
            .HasForeignKey(x => x.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.ShopLocation)
            .WithMany()
            .HasForeignKey(x => x.ShopLocationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.ShopLocationId, x.EmployeeWorkMode, x.StartUtc, x.EndUtc });
        builder.HasIndex(x => x.EmployeeId);
    }
}