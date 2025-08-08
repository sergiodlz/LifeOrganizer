using System;
using LifeOrganizer.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LifeOrganizer.Data.Configurations;

public class BudgetPeriodTransactionConfiguration: IEntityTypeConfiguration<BudgetPeriodTransaction>
{
    public void Configure(EntityTypeBuilder<BudgetPeriodTransaction> builder)
    {
        builder.HasKey(bpt => bpt.Id);

        builder.HasOne(bpt => bpt.BudgetPeriod)
            .WithMany(bp => bp.BudgetPeriodTransactions)
            .HasForeignKey(bpt => bpt.BudgetPeriodId);

        builder.HasOne(bpt => bpt.Transaction)
            .WithMany(t => t.BudgetPeriodTransactions)
            .HasForeignKey(bpt => bpt.TransactionId);

        builder.HasIndex(bpt => new { bpt.BudgetPeriodId, bpt.TransactionId }).IsUnique();
    }
}