using LteVideoPlayer.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace LteVideoPlayer.Api.Persistance.Repositories
{
    public interface IRepositorySetup
    {

    }

    public interface IRepository<T> : IRepositorySetup where T : class
    {
        Task<EntityEntry<T>> AddAsync(T entity);
        Task AddRangeAsync(IEnumerable<T> entities);
        void UpdateState(T entity);
        EntityEntry<T> Update(T entity);
        void UpdateRange(IEnumerable<T> entities);
        EntityEntry<T> Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);
        void Detach(T entity);
        void ClearTracking();
        Task<int> SaveChangesAsync();
    }

    public interface IBaseRepositorySetup
    {

    }

    public abstract class BaseRepository<T> : IBaseRepositorySetup, IRepository<T> where T : class
    {
        private readonly DbContext _dbContext;

        protected BaseRepository(DbContext dbContext) => _dbContext = dbContext;

        public async Task<EntityEntry<T>> AddAsync(T entity)
            => await _dbContext.Set<T>().AddAsync(entity);

        public async Task AddRangeAsync(IEnumerable<T> entities)
            => await _dbContext.Set<T>().AddRangeAsync(entities);

        public void UpdateState(T entity)
            => _dbContext.Entry(entity).State = EntityState.Modified;

        public EntityEntry<T> Update(T entity)
            => _dbContext.Set<T>().Update(entity);

        public void UpdateRange(IEnumerable<T> entities)
            => _dbContext.Set<T>().UpdateRange(entities);

        public EntityEntry<T> Remove(T entity)
            => _dbContext.Set<T>().Remove(entity);

        public void RemoveRange(IEnumerable<T> entities)
            => _dbContext.Set<T>().RemoveRange(entities);

        public void Detach(T entity)
            => _dbContext.Entry(entity).State = EntityState.Detached;

        public void ClearTracking()
            => _dbContext.ChangeTracker.Clear();

        public async Task<int> SaveChangesAsync()
            => await _dbContext.SaveChangesAsync();

        protected async Task<T?> GetFirstOrDefaultAsync(IQueryable<T> query)
            => await query.FirstOrDefaultAsync();

        protected async Task<T?> GetFirstOrDefaultAsync(Expression<Func<T, bool>> predicate, Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null, List<Func<IQueryable<T>, IIncludableQueryable<T, object>>?>? includes = null, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, bool tracking = true)
            => await GenerateQuery(predicate, include, includes, orderBy, null, tracking).FirstOrDefaultAsync();

        protected async Task<T?> GetSingleOrDefaultAsync(IQueryable<T> query)
            => await query.SingleOrDefaultAsync();

        protected async Task<T?> GetSingleOrDefaultAsync(Expression<Func<T, bool>> predicate, Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null, List<Func<IQueryable<T>, IIncludableQueryable<T, object>>?>? includes = null, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, bool tracking = true)
            => await GenerateQuery(predicate, include, includes, orderBy, null, tracking).SingleOrDefaultAsync();

        protected async Task<List<T>> GetAsync(Expression<Func<T, bool>>? predicate = null, Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null, List<Func<IQueryable<T>, IIncludableQueryable<T, object>>?>? includes = null, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, int? take = null, bool tracking = true)
            => await GenerateQuery(predicate, include, includes, orderBy, take, tracking).ToListAsync();

        protected async Task<Paginate<T>> GetPaginateAsync(PaginateFilterDto paginateFilter, IQueryable<T> query)
        {
            var paginate = new Paginate<T>
            {
                PageIndex = paginateFilter.PageIndex,
                PageItemCount = paginateFilter.PageItemCount,
                TotalItemCount = await query.CountAsync()
            };

            if (paginateFilter.IsPaginated)
            {
                paginate.TotalPages = paginate.TotalItemCount / paginateFilter.PageItemCount + (
                    paginate.TotalItemCount % paginateFilter.PageItemCount > 0
                        ? 1
                        : 0);

                paginate.Items = query
                    .Skip(paginateFilter.PageIndex * paginateFilter.PageItemCount)
                    .Take(paginateFilter.PageItemCount);
            }
            else
            {
                paginate.TotalPages = 1;
                paginate.Items = query;
            }

            return paginate;
        }

        protected async Task<Paginate<T>> GetPaginateAsync(PaginateFilterDto paginateFilter, Expression<Func<T, bool>>? predicate = null, Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null, List<Func<IQueryable<T>, IIncludableQueryable<T, object>>?>? includes = null, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, bool tracking = true)
            => await GetPaginateAsync(paginateFilter, GenerateQuery(predicate, include, includes, orderBy, null, tracking));

        protected async Task<int> RawSqlAsync(string sql) =>
            await _dbContext.Database.ExecuteSqlRawAsync(sql);

        protected IQueryable<T> GenerateQuery(Expression<Func<T, bool>>? predicate = null, Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null, List<Func<IQueryable<T>, IIncludableQueryable<T, object>>?>? includes = null, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, int? take = null, bool tracking = true)
        {
            var query = _dbContext.Set<T>().AsQueryable();

            if (predicate != null)
                query = query.Where(predicate);
            if (include != null)
                query = include(query);
            if (includes != null)
            {
                foreach (var inc in includes)
                {
                    if (inc != null)
                        query = inc(query);
                }
            }
            if (orderBy != null)
                query = orderBy(query);
            if (take != null)
                query = query.Take(take.Value);
            if (!tracking)
                query = query.AsNoTracking();

            var a = query.ToQueryString();

            return query;
        }
    }
}
