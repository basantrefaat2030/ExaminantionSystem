using ExaminantionSystem.Entities;
using ExaminantionSystem.Entities.Wrappers;
using ExaminantionSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
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
        private DbSet<TEntity> _dbSet;
        public Repository(ExaminationContext executionContex)
        {
            _executionContext = executionContex;
            _dbSet = executionContex.Set<TEntity>();

        }

        public IQueryable<TEntity> GetAll()
        {
            return _dbSet.Where(e => !e.IsDeleted);
        }

        public async Task<TEntity> GetByIdAsync(int id)
        {
            var query = await _dbSet.FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted );
            return query;
        }

        public async Task<TEntity> GetByIdTrackingAsync(int id)
        {
            var res = await _dbSet.AsTracking().FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted);
            return res;
        }

        public async Task AddAsync(TEntity entity)
        {
            entity.CreatedAt = DateTime.UtcNow;
            await _dbSet.AddAsync(entity);

        }

        public async Task DeleteAsync(int id)
        {
            await _dbSet.Where(e => e.Id == id && !e.IsDeleted)
             .ExecuteUpdateAsync(setters => setters
             .SetProperty(e => e.IsDeleted, true)
             .SetProperty(e => e.DeletedAt, DateTime.UtcNow));
        }

        public IQueryable<TEntity> GetAll(Expression<Func<TEntity, bool>> expression)
        {
            var query = GetAll();
            return query.Where(expression); ;
        }

        public async Task AddRangeAsync(IEnumerable<TEntity> entities)
        {
            await _dbSet.AddRangeAsync(entities);
        }


        public async Task UpdateRangeAsync(Expression<Func<TEntity, bool>> predicate, Expression<Func<SetPropertyCalls<TEntity>, SetPropertyCalls<TEntity>>> setPropertyCalls)
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

        //public void UpdateInclude(TEntity entity, params string[] modifiedParams)
        //{
        //    if (!_dbSet.Any(x => x.Id == entity.Id))
        //    { return; }

        //    var local = _dbSet.Local.FirstOrDefault(x => x.Id == entity.Id);
        //    EntityEntry entityEntry;

        //    if (local == null)
        //    {
        //        entityEntry = _executionContext.Entry(entity);
        //    }
        //    else
        //    {
        //        entityEntry = _executionContext.ChangeTracker.Entries<TEntity>().FirstOrDefault(x => x.Entity.Id == entity.Id);
        //    }

        //    foreach (var prop in entityEntry.Properties)
        //    {
        //        if (modifiedParams.Contains(prop.Metadata.Name))
        //        {
        //            prop.CurrentValue = entity.GetType().GetProperty(prop.Metadata.Name).GetValue(entity);
        //            prop.IsModified = true;
        //        }
        //    }
        //   // _executionContext.SaveChanges();
        //}

        public async Task<int> UpdateAsync(TEntity entity)
        {
            var idProperty = typeof(TEntity).GetProperty("Id");
            var entityId = idProperty?.GetValue(entity);
            if (entityId == null)
                return 0;

            // Parameter for the setters in the expression (equivalent to 'setters => ...')
            var setParameter = Expression.Parameter(typeof(SetPropertyCalls<TEntity>), "setters");

            // Start with the initial setters expression
            var currentExpression = (Expression)setParameter;

            // Get the open generic SetProperty method
            var openSetPropMethod = typeof(SetPropertyCalls<TEntity>).GetMethods()
                .First(m => m.Name == "SetProperty" && m.IsGenericMethod);

            // Get all updatable properties (excluding Id)
            var properties = typeof(TEntity).GetProperties()
                .Where(p => p.Name != "Id" && p.CanRead && p.CanWrite);

            foreach (var property in properties)
            {
                var propName = property.Name;
                var value = property.GetValue(entity);

                // Build the property selector: e => EF.Property<object>(e, propName)
                var eParam = Expression.Parameter(typeof(TEntity), "e");
                var efPropertyCall = Expression.Call(
                    typeof(EF),
                    nameof(EF.Property),
                    new[] { typeof(object) },
                    eParam,
                    Expression.Constant(propName)
                );
                var propertyLambda = Expression.Lambda<Func<TEntity, object>>(efPropertyCall, eParam);

                // Close the generic method for object type
                var closedMethod = openSetPropMethod.MakeGenericMethod(typeof(object));

                // Chain the SetProperty call: current.SetProperty(propertyLambda, value)
                currentExpression = Expression.Call(
                    currentExpression,
                    closedMethod,
                    propertyLambda,
                    Expression.Constant(value, typeof(object))
                );
            }

            // Handle UpdatedAt if it exists
            var updatedAtProperty = typeof(TEntity).GetProperty("UpdatedAt");
            if (updatedAtProperty != null)
            {
                var propName = "UpdatedAt";
                var value = DateTime.UtcNow;

                // Build the property selector: e => EF.Property<object>(e, "UpdatedAt")
                var eParam = Expression.Parameter(typeof(TEntity), "e");
                var efPropertyCall = Expression.Call(
                    typeof(EF),
                    nameof(EF.Property),
                    new[] { typeof(object) },
                    eParam,
                    Expression.Constant(propName)
                );
                var propertyLambda = Expression.Lambda<Func<TEntity, object>>(efPropertyCall, eParam);

                // Close the generic method for object type
                var closedMethod = openSetPropMethod.MakeGenericMethod(typeof(object));

                // Chain the SetProperty call for UpdatedAt
                currentExpression = Expression.Call(
                    currentExpression,
                    closedMethod,
                    propertyLambda,
                    Expression.Constant(value, typeof(object))
                );
            }

            // Build the final lambda: setters => [chained SetProperty calls]
            var lambda = Expression.Lambda<Func<SetPropertyCalls<TEntity>, SetPropertyCalls<TEntity>>>(
                currentExpression,
                setParameter
            );

            // Execute the update using the built expression
            return await _dbSet
                .Where(e => EF.Property<object>(e, "Id").Equals(entityId))
                .ExecuteUpdateAsync(lambda);
        }

        public async Task BegainTransactionAsync()
        {
            await _executionContext.Database.BeginTransactionAsync();
        }
        public async Task CommitTransactionAsync()
        {
            await _executionContext.Database.CommitTransactionAsync();
        }
        public async Task RollbackTransactionAsync()
        {
            await _executionContext.Database.RollbackTransactionAsync();
        }

    }
}
