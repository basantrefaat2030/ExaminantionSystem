using ExaminantionSystem.Entities.Wrappers;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace ExaminantionSystem.Infrastructure
{
    public interface IRepository<TEntity>
    {

        IQueryable<TEntity> GetAll();
        IQueryable<TEntity> GetAll(Expression<Func<TEntity, bool>> expression);
        Task<TEntity> GetByIdAsync(int id);
        Task<TEntity> GetByIdTrackingAsync(int id);

        Task AddAsync(TEntity entity);

        Task AddRangeAsync(IEnumerable<TEntity> entities);
        Task<int> UpdateAsync(TEntity);

        Task UpdateRangeAsync(Expression<Func<TEntity, bool>> predicate, Expression<Func<SetPropertyCalls<TEntity>, SetPropertyCalls<TEntity>>> setPropertyCalls);
        Task DeleteAsync(int id);
        Task DeleteRangeAsync(IEnumerable<TEntity> entities);

        // Tracking control
        //IQueryable<TEntity> AsTracking(IQueryable<TEntity> query);
        Task SaveChangesAsync();

    }
}
