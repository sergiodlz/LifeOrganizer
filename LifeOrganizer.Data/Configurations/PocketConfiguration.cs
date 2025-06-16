using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LifeOrganizer.Data.Entities;

namespace LifeOrganizer.Data.Configurations
{
    public class PocketConfiguration : IEntityTypeConfiguration<Pocket>
    {
        public void Configure(EntityTypeBuilder<Pocket> builder)
        {
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Name).IsRequired().HasMaxLength(100);
            builder.HasOne(p => p.Account)
                   .WithMany(a => a.Pockets)
                   .HasForeignKey(p => p.AccountId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
