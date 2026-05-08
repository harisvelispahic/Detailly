using Detailly.Domain.Entities.Booking;

namespace Detailly.Infrastructure.Database.Configurations.Booking;
public class ServicePackageItemAssignmentConfiguration: IEntityTypeConfiguration<ServicePackageItemAssignmentEntity>
{
    public void Configure(EntityTypeBuilder<ServicePackageItemAssignmentEntity> builder)
    {
        // ---------- ServicePackageItemAssignment (N:M join table) ----------

        builder
            .ToTable("ServicePackageItemAssignments");

        builder
            .HasKey(e => e.Id); // surrogate key
        
        builder
            .HasOne(e => e.ServicePackage)
            .WithMany(p => p.ServicePackageItemAssignments)
            .HasForeignKey(e => e.ServicePackageId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder
            .HasOne(e => e.ServicePackageItem)
            .WithMany(i => i.ServicePackageItemAssignments)
            .HasForeignKey(e => e.ServicePackageItemId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
