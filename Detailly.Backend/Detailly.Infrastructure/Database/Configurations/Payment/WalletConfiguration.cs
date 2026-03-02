using Detailly.Domain.Entities.Payment;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Detailly.Infrastructure.Database.Configurations.Payment;

public sealed class WalletConfiguration
    : IEntityTypeConfiguration<WalletEntity>
{
    public void Configure(EntityTypeBuilder<WalletEntity> builder)
    {
        builder.ToTable("Wallets");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Balance)
            .IsRequired();

        builder.HasOne(x => x.ApplicationUser)
            .WithOne(x => x.Wallet)
            .HasForeignKey<WalletEntity>(x => x.ApplicationUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
