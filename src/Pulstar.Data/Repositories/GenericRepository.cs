namespace Pulstar.Data.Repositories
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Pulstar.Data;
    using Pulstar.Data.Interfaces;

    public class GenericRepository<T> : IRepository<T>
        where T : class, IEntity
    {
        private readonly PulstarDbContext _dbContext;
        private bool _disposed;

        public GenericRepository(PulstarDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<T> AddAsync(T entity)
        {
            entity.CreatedOn = DateTime.UtcNow;

            await _dbContext.Set<T>().AddAsync(entity);
            var savedEntries = await _dbContext.SaveChangesAsync();

            return entity;
        }

        public IQueryable<T> All()
        {
            return GetWithNavigationProperties().Where(m => !m.IsDeleted);
        }

        public IQueryable<T> AllWithDeleted()
        {
            return GetWithNavigationProperties();
        }

        public async Task DeleteAsync(object id)
        {
            var entity = await GetByIdAsync(id);
            entity.IsDeleted = true;
            entity.DeletedOn = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(T entity)
        {
            entity.IsDeleted = true;
            entity.DeletedOn = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();
        }

        public async Task HardDeleteAsync(T entity)
        {
            _dbContext.Set<T>().Remove(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task HardDeleteAsync(object id)
        {
            var entity = await GetByIdAsync(id);
            _dbContext.Set<T>().Remove(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<T> GetByIdAsync(object id)
        {
            return await _dbContext.Set<T>().FindAsync(id);
        }

        public async Task<T> UpdateAsync(T entity)
        {
            entity.ModifiedOn = DateTime.UtcNow;
            _dbContext.Set<T>().Update(entity);
            await _dbContext.SaveChangesAsync();

            return entity;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                _dbContext?.Dispose();
            }

            _disposed = true;
        }

        private IQueryable<T> GetWithNavigationProperties()
        {
            var query = _dbContext.Set<T>().AsQueryable();
            foreach (var property in _dbContext.Model.FindEntityType(typeof(T)).GetNavigations())
            {
                query = query.Include(property.Name);
            }

            return query.AsNoTracking();
        }
    }
}
