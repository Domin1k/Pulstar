namespace Pulstar.Data.Interfaces
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    public interface IRepository<TEntity> : IDisposable
        where TEntity : class
    {
        IQueryable<TEntity> All();

        IQueryable<TEntity> AllWithDeleted();
        
        Task<TEntity> GetByIdAsync(object id);

        void Add(TEntity entity);

        void Update(TEntity entity);

        Task DeleteAsync(object id);
        
        void HardDelete(TEntity entity);

        Task<int> SaveChangesAsync();
    }
}
