
using Detailly.Domain.Entities.Booking;

namespace Detailly.Infrastructure.Database.Configurations.Identity;

public sealed class ApplicationUserEntityConfiguration
    : IEntityTypeConfiguration<ApplicationUserEntity>
{
    public void Configure(EntityTypeBuilder<ApplicationUserEntity> b)
    {
        b.ToTable("ApplicationUsers");

        b.HasKey(x => x.Id);

        b.HasIndex(x => x.Email)
            .IsUnique();

        b.Property(x => x.Email)
            .IsRequired()
            .HasMaxLength(200);

        b.Property(x => x.PasswordHash)
            .IsRequired();

        // Roles
        b.Property(x => x.IsAdmin)
            .HasDefaultValue(false);

        b.Property(x => x.IsManager)
            .HasDefaultValue(false);

        b.Property(x => x.IsEmployee)
            .HasDefaultValue(true); // Default: regular user

        b.Property(x => x.TokenVersion)
            .HasDefaultValue(0);

        b.Property(x => x.IsEnabled)
            .HasDefaultValue(true);

        // =========================
        // Navigation relationships
        // =========================

        // Customer → Bookings
        b.HasMany(x => x.Bookings)
            .WithOne(x => x.ApplicationUser)
            .HasForeignKey(x => x.ApplicationUserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Employee → Assigned Bookings
        // (no collection on ApplicationUser side)
        b.HasMany<BookingEntity>()
            .WithOne(x => x.Employee)
            .HasForeignKey(x => x.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        // Refresh tokens
        b.HasMany(x => x.RefreshTokens)
            .WithOne(x => x.ApplicationUser)
            .HasForeignKey(x => x.ApplicationUserId);
    }
}
