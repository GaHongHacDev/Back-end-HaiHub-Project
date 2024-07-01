using Hairhub.Domain.Specifications;
using Hairhub.Service.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;
using Hairhub.Domain.Specifications;


namespace Hairhub.Infrastructure.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly DbContext Context;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(DbContext context)
        {
            Context = context;
            _dbSet = context.Set<T>();
        }

        public void Dispose()
        {
            Context?.Dispose();
        }

        #region Gett Async

        public virtual async Task<T> SingleOrDefaultAsync(
            Expression<Func<T, bool>> predicate = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, 
                        Func<IQueryable<T>, IIncludableQueryable<T, object>> 
                        include = null)
        {
            IQueryable<T> query = _dbSet;
            if (include != null) query = include(query);

            if (predicate != null) query = query.Where(predicate);

            if (orderBy != null) return await orderBy(query).AsNoTracking().FirstOrDefaultAsync();

            return await query.AsNoTracking().FirstOrDefaultAsync();
        }

        public virtual async Task<TResult> SingleOrDefaultAsync<TResult>(
            Expression<Func<T, TResult>> selector, Expression<Func<T, bool>> predicate = null, Func<IQueryable<T>, 
                            IOrderedQueryable<T>> orderBy = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null)
        {
            IQueryable<T> query = _dbSet;
            if (include != null) query = include(query);

            if (predicate != null) query = query.Where(predicate);

            if (orderBy != null) return await orderBy(query).AsNoTracking().Select(selector).FirstOrDefaultAsync();

            return await query.AsNoTracking().Select(selector).FirstOrDefaultAsync();
        }

        public virtual async Task<ICollection<T>> GetListAsync(
            Expression<Func<T, bool>> predicate = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, 
                        Func<IQueryable<T>, IIncludableQueryable<T, object>> 
                        include = null, int? take = null)
        {
            IQueryable<T> query = _dbSet;

            if (include != null) query = include(query);

            if (predicate != null) query = query.Where(predicate);

            if (take.HasValue)
            {
                query = query.Take(take.Value);
            }

            if (orderBy != null) return await orderBy(query).AsNoTracking().ToListAsync();

            return await query.AsNoTracking().ToListAsync();
        }

        public virtual async Task<ICollection<TResult>> GetListAsync<TResult>(
            Expression<Func<T, TResult>> selector, Expression<Func<T, bool>> predicate = null, Func<IQueryable<T>, 
                        IOrderedQueryable<T>> orderBy = null, Func<IQueryable<T>, IIncludableQueryable<T, object>> 
                        include = null)
        {
            IQueryable<T> query = _dbSet;

            if (include != null) query = include(query);

            if (predicate != null) query = query.Where(predicate);

            if (orderBy != null) return await orderBy(query).AsNoTracking().Select(selector).ToListAsync();

            return await query.Select(selector).ToListAsync();
        }

        public Task<IPaginate<T>> GetPagingListAsync(
                Expression<Func<T, bool>> predicate = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, 
                            Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null, 
                int page = 1,
                int size = 10)
        {
            IQueryable<T> query = _dbSet;
            if (include != null) query = include(query);
            if (predicate != null) query = query.Where(predicate);
            if (orderBy != null) return orderBy(query).ToPaginateAsync(page, size, 1);
            return query.AsNoTracking().ToPaginateAsync(page, size, 1);
        }

        public Task<IPaginate<TResult>> GetPagingListAsync<TResult>(
            Expression<Func<T, TResult>> selector, Expression<Func<T, bool>> predicate = null, Func<IQueryable<T>, 
                        IOrderedQueryable<T>> orderBy = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null, 
            int page = 1, int size = 10)
        {
            if (page <= 0) page = 1;
            if (size <= 0) size = 10;
            IQueryable<T> query = _dbSet;
            if (include != null) query = include(query);
            if (predicate != null) query = query.Where(predicate);
            if (orderBy != null) return orderBy(query).Select(selector).ToPaginateAsync(page, size, 1);
            return query.AsNoTracking().Select(selector).ToPaginateAsync(page, size, 1);
        }

        #endregion

        #region Insert

        public async Task InsertAsync(T entity)
        {
            if (entity == null) return;
            await _dbSet.AddAsync(entity);
        }

        public async Task InsertRangeAsync(IEnumerable<T> entities)
        {
            await _dbSet.AddRangeAsync(entities);
        }

        #endregion

        #region Update
        public void UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
        }

        public void UpdateRange(IEnumerable<T> entities)
        {
            _dbSet.UpdateRange(entities);
        }

        public void DeleteAsync(T entity)
        {
            _dbSet.Remove(entity);
        }

        public void DeleteRangeAsync(IEnumerable<T> entities)
        {
            _dbSet.RemoveRange(entities);
        }

        
        #endregion
    }
}
