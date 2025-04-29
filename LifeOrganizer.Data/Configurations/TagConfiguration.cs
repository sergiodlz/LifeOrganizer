using LifeOrganizer.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LifeOrganizer.Data.Configurations;

public class TagConfiguration : IEntityTypeConfiguration<Tag>
{
    public void Configure(EntityTypeBuilder<Tag> builder)
    {
        builder.HasIndex(t => new { t.UserId, t.Name }).IsUnique();
        builder.HasMany(t => t.Transactions)
            .WithMany(tr => tr.Tags);
    }
}
