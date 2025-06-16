using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using LifeOrganizer.Data.Entities;

namespace LifeOrganizer.Data;
public class LifeOrganizerContext : DbContext
{
    public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;
    public LifeOrganizerContext(DbContextOptions<LifeOrganizerContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new Configurations.AccountConfiguration());
        modelBuilder.ApplyConfiguration(new Configurations.CategoryConfiguration());
        modelBuilder.ApplyConfiguration(new Configurations.SubcategoryConfiguration());
        modelBuilder.ApplyConfiguration(new Configurations.TagConfiguration());
        modelBuilder.ApplyConfiguration(new Configurations.TransactionConfiguration());
        modelBuilder.ApplyConfiguration(new Configurations.UserConfiguration());
        modelBuilder.ApplyConfiguration(new Configurations.PocketConfiguration());

        // Global query filter for soft delete
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
            {
                var parameter = Expression.Parameter(entityType.ClrType, "e");
                var isDeletedProperty = Expression.Property(parameter, nameof(BaseEntity.IsDeleted));
                var filter = Expression.Lambda(
                    Expression.Equal(isDeletedProperty, Expression.Constant(false)),
                    parameter
                );
                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(filter);
            }
        }
    }
}
