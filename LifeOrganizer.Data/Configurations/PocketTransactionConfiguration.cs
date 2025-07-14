using LifeOrganizer.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LifeOrganizer.Data.Configurations;

public class PocketTransactionConfiguration : IEntityTypeConfiguration<PocketTransaction>
{
    public void Configure(EntityTypeBuilder<PocketTransaction> builder)
    {
        builder.HasOne(t => t.Pocket)
            .WithMany(p => p.Transactions)
            .HasForeignKey(t => t.PocketId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

