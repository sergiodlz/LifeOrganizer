using System.Collections;
using LifeOrganizer.Data.Entities;
using LifeOrganizer.Data.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace LifeOrganizer.Data.UnitOfWorkPattern
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly LifeOrganizerContext _context;
        private Hashtable? _repositories;

        public UnitOfWork(LifeOrganizerContext context)
        {
            _context = context;
        }

        public IRepository<TEntity> Repository<TEntity>() where TEntity : BaseEntity
        {
            if (_repositories == null)
                _repositories = new Hashtable();

            var type = typeof(TEntity).Name;

            if (!_repositories.ContainsKey(type))
            {
                var repositoryType = typeof(Repository<>);
                var repositoryInstance = Activator.CreateInstance(repositoryType.MakeGenericType(typeof(TEntity)), _context);
                _repositories.Add(type, repositoryInstance!);
            }

            return (IRepository<TEntity>)_repositories[type]!;
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _context.Database.BeginTransactionAsync();
        }

        public async Task<int> SaveChangesAsync(System.Threading.CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
