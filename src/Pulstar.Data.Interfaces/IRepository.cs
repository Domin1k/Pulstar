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

        Task<TEntity> AddAsync(TEntity entity);

        Task<TEntity> UpdateAsync(TEntity entity);

        Task DeleteAsync(object id);

        Task DeleteAsync(TEntity entity);

        Task HardDeleteAsync(object id);

        Task HardDeleteAsync(TEntity entity);
    }
}
