using LifeOrganizer.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LifeOrganizer.Data.Configurations;

public class BudgetConfiguration: IEntityTypeConfiguration<Budget>
{
    public void Configure(EntityTypeBuilder<Budget> builder)
    {
        builder.HasIndex(a => a.Id).IsUnique();
        builder.HasMany(a => a.Rules)
            .WithOne(r => r.Budget)
            .HasForeignKey(a => a.BudgetId);
        builder.HasMany(a => a.Periods)
            .WithOne(p => p.Budget)
            .HasForeignKey(a => a.BudgetId);
    }
}