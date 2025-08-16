using ExaminantionSystem.Entities.Wrappers;
using System.Linq.Expressions;

namespace ExaminantionSystem.Infrastructure
{
    public interface IRepository<TEntity>
    {
        IQueryable<TEntity> GetAll(bool includeDeleted = false);
        Task<TEntity?> GetByIdAsync(int id , bool includeDeleted = false);
        Task AddAsync(TEntity entity);
        Task UpdateAsync(TEntity entity);
        Task DeleteAsync(int id);

        IQueryable<TEntity> Find(Expression<Func<TEntity, bool>> expression, bool includeDeleted = false);

        Task SaveChangesAsync();

    }
}
