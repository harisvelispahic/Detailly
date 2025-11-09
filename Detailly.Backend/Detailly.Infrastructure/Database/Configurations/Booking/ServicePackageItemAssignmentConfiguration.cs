
using Detailly.Domain.Entities.Booking;

namespace Detailly.Infrastructure.Database.Configurations.Booking;
public class ServicePackageItemAssignmentConfiguration: IEntityTypeConfiguration<ServicePackageItemAssignmentEntity>
{
    public void Configure(EntityTypeBuilder<ServicePackageItemAssignmentEntity> builder)
    {
        // ---------- ServicePackageItemAssignment (N:M medjutabela) ----------

        builder
            .ToTable("ServicePackageItemAssignments"); // ime tabele

        builder
            .HasKey(e => e.Id); // surogat ključ
        
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
