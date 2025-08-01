using LifeOrganizer.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LifeOrganizer.Data.Configurations;

public class BudgetRuleConfiguration: IEntityTypeConfiguration<BudgetRule>
{
    public void Configure(EntityTypeBuilder<BudgetRule> builder)
    {
        builder.HasIndex(a => new { a.BudgetId, a.CategoryId, a.SubcategoryId, a.TagId }).IsUnique();
        builder.HasOne(a => a.Category)
            .WithMany()
            .HasForeignKey(a => a.CategoryId);
        builder.HasOne(a => a.Subcategory)
            .WithMany()
            .HasForeignKey(a => a.SubcategoryId);
        builder.HasOne(a => a.Tag)
            .WithMany()
            .HasForeignKey(a => a.TagId);
    }
}
