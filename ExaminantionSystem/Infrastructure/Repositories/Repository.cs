using ExaminantionSystem.Entities;
using ExaminantionSystem.Entities.Wrappers;
using ExaminantionSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using System.Linq.Expressions;
using System.Threading;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace ExaminantionSystem.Infrastructure.Repositories
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : BaseEntity
    {
        private ExaminationContext _executionContext;
        private DbSet <TEntity> _dbSet;
        public Repository(ExaminationContext executionContex) 
        {
            _executionContext = executionContex;
            _dbSet = executionContex.Set<TEntity>();

        }

        public IQueryable<TEntity> GetAll()
        {
            return _dbSet.Where(e => !e.IsDeleted);
        }

        public async Task<TEntity?> GetByIdAsync(int id , bool trackChanges = false)
        {
            var query = _dbSet.Where(e => !e.IsDeleted);

            query = query.Where(e => e.Id == id);
            return trackChanges ? await query.AsTracking().FirstOrDefaultAsync() :await  query.AsNoTracking().FirstOrDefaultAsync();
        }

        public async Task AddAsync(TEntity entity)
        {
            await _dbSet.AddAsync(entity);
            
        }

        public async Task UpdateAsync(TEntity entity)
        {
             _dbSet.Entry(entity).State = EntityState.Modified;
        }

        public async Task DeleteAsync(TEntity entity)
        {
            // Soft delete
            entity.IsDeleted = true;
            entity.DeletedAt = DateTime.UtcNow;
            await UpdateAsync(entity);
        }

        public IQueryable<TEntity> GetAll(Expression<Func<TEntity, bool>> expression)
        {
            var query =  GetAll();
            return query.Where(expression); ;
        }

        //public IQueryable<TEntity> AsTracking(IQueryable<TEntity> query)
        //{
        //    return query.AsTracking();
        //}

        //public IQueryable<TEntity> AsNoTracking(IQueryable<TEntity> query)
        //{
        //    return query.AsNoTracking();
        //}
        public async Task AddRangeAsync(IEnumerable<TEntity> entities)
        {
            await _dbSet.AddRangeAsync(entities);
        }

        public async Task<int> UpdateAsync(Expression<Func<TEntity, bool>> predicate, Expression<Func<SetPropertyCalls<TEntity>, SetPropertyCalls<TEntity>>> setPropertyCalls)
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            if (setPropertyCalls == null)
                throw new ArgumentNullException(nameof(setPropertyCalls));

            return await _dbSet
                .Where(predicate)
                .ExecuteUpdateAsync(setPropertyCalls);
        }
        public async Task UpdateRangeAsync(Expression<Func<TEntity, bool>> predicate,Expression<Func<SetPropertyCalls<TEntity>, SetPropertyCalls<TEntity>>> setPropertyCalls)
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            if (setPropertyCalls == null)
                throw new ArgumentNullException(nameof(setPropertyCalls));

             await _dbSet.Where(predicate).ExecuteUpdateAsync(setPropertyCalls);
        }

        // Bulk delete by entities using ExecuteUpdate (Highly efficient)
        public async Task DeleteRangeAsync(IEnumerable<TEntity> entities)
        {
            if (!entities.Any()) return;

            var ids = entities.Select(e => e.Id).ToList();
            await DeleteRangByIds(ids);
        }

        private async Task DeleteRangByIds(IEnumerable<int> ids)
        {
            if (!ids.Any()) return;

            await _dbSet
                .Where(e => ids.Contains(e.Id))
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(e => e.IsDeleted, true)
                    .SetProperty(e => e.DeletedAt, DateTime.UtcNow));
        }

        public async Task SaveChangesAsync()
        {
            await _executionContext.SaveChangesAsync();
        }

        public async  Task<int> UpdateAsync(int id, Expression<Func<SetPropertyCalls<TEntity>, SetPropertyCalls<TEntity>>> setPropertyCalls)
        {
            return await UpdateAsync(e => e.Id == id, setPropertyCalls);
        }
    }
}
