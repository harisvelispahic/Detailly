using Detailly.Domain.Entities.Payment;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Detailly.Infrastructure.Database.Configurations.Payment;

public sealed class WalletConfiguration
    : IEntityTypeConfiguration<WalletEntity>
{
    public void Configure(EntityTypeBuilder<WalletEntity> b)
    {
        b.ToTable("Wallets");

        b.HasKey(x => x.Id);

        b.Property(x => x.Balance)
            .IsRequired();

        b.HasOne(x => x.ApplicationUser)
            .WithOne(x => x.Wallet)
            .HasForeignKey<WalletEntity>(x => x.ApplicationUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
