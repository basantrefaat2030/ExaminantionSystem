using ExaminantionSystem.Entities;
using ExaminantionSystem.Entities.Wrappers;
using ExaminantionSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using System.Linq.Expressions;
using System.Threading;

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

        public IQueryable<TEntity> GetAll(bool includeDeleted = false)
        {
            return includeDeleted
          ? _dbSet.IgnoreQueryFilters()
          : _dbSet.Where(e => !e.IsDeleted);
        }

        public async Task<TEntity?> GetByIdAsync(int id , bool includeDeleted = false)
        {
            return includeDeleted
           ? await _dbSet.IgnoreQueryFilters().FirstOrDefaultAsync(e => e.Id == id)
           : await _dbSet.FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted);
        }

        public async Task AddAsync(TEntity entity)
        {
            await _dbSet.AddAsync(entity);
            
        }

        public async Task UpdateAsync(TEntity entity)
        {
             _dbSet.Entry(entity).State = EntityState.Modified;
        }

        //soft delete
        public async Task DeleteAsync(int id)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity != null)
            {
                entity.IsDeleted = true;
                entity.DeletedAt = DateTime.UtcNow;
            }
        }
        public IQueryable<TEntity> Find(Expression<Func<TEntity, bool>> expression, bool includeDeleted = false)
        {
            var query =  _dbSet.Where(expression);

            if (!includeDeleted && typeof(AuditEntity).IsAssignableFrom(typeof(TEntity)))
            {
                query = query.Where(e => !e.IsDeleted);
            }

            return query;
        }
        public async Task SaveChangesAsync()
        {
           await _executionContext.SaveChangesAsync();
        }
    }
}
