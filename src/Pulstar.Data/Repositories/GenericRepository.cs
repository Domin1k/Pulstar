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
        private readonly DbSet<T> _dbSet;
        private bool _disposed;

        public GenericRepository(PulstarDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _dbSet = _dbContext.Set<T>();
        }
        
        public IQueryable<T> All()
        {
            return GetWithNavigationProperties().Where(m => !m.IsDeleted);
        }

        public IQueryable<T> AllWithDeleted()
        {
            return GetWithNavigationProperties();
        }
        
        public async Task<T> GetByIdAsync(object id)
        {
            return await _dbContext.Set<T>().FindAsync(id);
        }
        
        public virtual void Add(T entity)
        {
            _dbContext.Entry(entity).State = EntityState.Added;
            _dbContext.Add(entity);
        }

        public virtual void Update(T entity)
        {
            var entry = _dbContext.Entry(entity);
            if (entry.State == EntityState.Detached)
            {
                _dbSet.Attach(entity);
            }

            entry.State = EntityState.Modified;
        }

        public virtual void Delete(T entity)
        {
            entity.IsDeleted = true;
            entity.DeletedOn = DateTime.UtcNow;
            var entry = _dbContext.Entry(entity);
            entry.State = EntityState.Modified;
        }

        public virtual async Task DeleteAsync(object id)
        {
            var entity = await GetByIdAsync(id);
            entity.IsDeleted = true;
            entity.DeletedOn = DateTime.UtcNow;
            Update(entity);
        }

        public async Task<int> SaveChangesAsync()
        {
            var result = await _dbContext.SaveChangesAsync();
            return result;
        }
                
        public void HardDelete(T entity)
            => _dbSet.Remove(entity);

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
