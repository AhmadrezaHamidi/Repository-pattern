using DatatAccess.Ef.Common;
using DatatAccess.Ef.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DatatAccess.Ef
{
    public class Repository<TEntity> : IRepository<TEntity>
           where TEntity : class

    {
        public DbContext Context { get; }

        public Repository(DbContext context)
        {
            Context = context;
        }

        #region Get With Options

        public IQueryable<TEntity> Query(Expression<Func<TEntity, bool>> predicate = null)
        {
            return predicate == null ? Context.Set<TEntity>() : Context.Set<TEntity>().Where(predicate);
        }

        public IEnumerable<TEntity> GetAll()
        {
            return Context.Set<TEntity>().ToList();
        }

        public IQueryable<TEntity> GetAllData()
        {
            return Context.Set<TEntity>();
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await Context.Set<TEntity>().ToListAsync();
        }

        public bool HasEntity(Expression<Func<TEntity, bool>> predicate)
        {
            return Context.Set<TEntity>().Any(predicate);
        }

        public async Task<bool> HasEntityAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await Context.Set<TEntity>().AnyAsync(predicate);
        }

        public TEntity GetEntity(Expression<Func<TEntity, bool>> predicate)
        {
            return Context.Set<TEntity>().SingleOrDefault(predicate);
        }

        public async Task<TEntity> GetEntityAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await Context.Set<TEntity>().SingleOrDefaultAsync(predicate);
        }

        public TEntity GetLastEntity(Expression<Func<TEntity, bool>> predicate)
        {
            return Context.Set<TEntity>().LastOrDefault(predicate);
        }

        public async Task<TEntity> GetLastEntityAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await Context.Set<TEntity>().LastOrDefaultAsync(predicate);
        }

        public TEntity FirstOrDefault()
        {
            return Context.Set<TEntity>().FirstOrDefault();
        }

        public async Task<TEntity> FirstOrDefaultAsync()
        {
            return await Context.Set<TEntity>().FirstOrDefaultAsync();
        }

        public TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            return Context.Set<TEntity>().FirstOrDefault(predicate);
        }

        public async Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await Context.Set<TEntity>().FirstOrDefaultAsync(predicate);
        }

        public TEntity First()
        {
            return Context.Set<TEntity>().First();
        }

        public async Task<TEntity> FirstAsync()
        {
            return await Context.Set<TEntity>().FirstAsync();
        }

        public TEntity First(Expression<Func<TEntity, bool>> predicate)
        {
            return Context.Set<TEntity>().First(predicate);
        }

        public async Task<TEntity> FirstAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await Context.Set<TEntity>().FirstAsync(predicate);
        }

        public async Task<EntityEntry<TEntity>> InsertAsync(TEntity entity)
        {
            return await Context.Set<TEntity>().AddAsync(entity);
        }

        public async Task AddRangeAsync(List<TEntity> entities)
        {
            await Context.Set<TEntity>().AddRangeAsync(entities);
        }

        public TEntity Last()
        {
            return Context.Set<TEntity>().Last();
        }

        public async Task<TEntity> LastAsync()
        {
            return await Context.Set<TEntity>().LastAsync();
        }

        public TEntity Last(Expression<Func<TEntity, bool>> predicate)
        {
            return Context.Set<TEntity>().Last(predicate);
        }

        public async Task<TEntity> LastAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await Context.Set<TEntity>().LastAsync(predicate);
        }

        public TEntity LastOrDefault()
        {
            return Context.Set<TEntity>().LastOrDefault();
        }

        public async Task<TEntity> LastOrDefaultAsync()
        {
            return await Context.Set<TEntity>().LastOrDefaultAsync();
        }

        public TEntity LastOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            return Context.Set<TEntity>().Last(predicate);
        }

        public async Task<TEntity> LastOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await Context.Set<TEntity>().LastAsync(predicate);
        }

        public TEntity SingleOrDefault()
        {
            return Context.Set<TEntity>().SingleOrDefault();
        }

        public async Task<TEntity> SingleOrDefaultAsync()
        {
            return await Context.Set<TEntity>().SingleOrDefaultAsync();
        }

        public TEntity SingleOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            return Context.Set<TEntity>().SingleOrDefault(predicate);
        }

        public async Task<TEntity> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await Context.Set<TEntity>().SingleOrDefaultAsync(predicate);
        }

        public TEntity Single()
        {
            return Context.Set<TEntity>().Single();
        }

        public async Task<TEntity> SingleAsync()
        {
            return await Context.Set<TEntity>().SingleAsync();
        }

        public TEntity Single(Expression<Func<TEntity, bool>> predicate)
        {
            return Context.Set<TEntity>().Single(predicate);
        }

        public async Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await Context.Set<TEntity>().SingleAsync(predicate);
        }

        public IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate)
        {
            return Context.Set<TEntity>().Where(predicate).ToList();
        }

        public async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await Context.Set<TEntity>().Where(predicate).ToListAsync();
        }

        public IEnumerable<TEntity> GetTop(int count)
        {
            return Context.Set<TEntity>().Skip(0).Take(count).ToList();
        }

        public async Task<IEnumerable<TEntity>> GetTopAsync(int count)
        {
            return await Context.Set<TEntity>().Skip(0).Take(count).ToListAsync();
        }

        public IEnumerable<TEntity> GetTop(int count, Expression<Func<TEntity, bool>> predicate)
        {
            return Context.Set<TEntity>().Where(predicate).Skip(0).Take(count).ToList();
        }

        public async Task<IEnumerable<TEntity>> GetTopAsync(int count, Expression<Func<TEntity, bool>> predicate)
        {
            return await Context.Set<TEntity>().Where(predicate).Skip(0).Take(count).ToListAsync();
        }

        public IEnumerable<TEntity> GetLast(int count)
        {
            return Context.Set<TEntity>().Skip(0).Take(count).ToList();
        }

        public async Task<IEnumerable<TEntity>> GetLastAsync(int count)
        {
            return await Context.Set<TEntity>().Skip(0).Take(count).ToListAsync();
        }

        public IEnumerable<TEntity> GetLast(int count, Expression<Func<TEntity, bool>> predicate)
        {
            return Context.Set<TEntity>().Where(predicate).Skip(0).Take(count).ToList();
        }

        public async Task<IEnumerable<TEntity>> GetLastAsync(int count, Expression<Func<TEntity, bool>> predicate)
        {
            return await Context.Set<TEntity>().Where(predicate).Skip(0).Take(count).ToListAsync();
        }

        #endregion Get With Options

        #region Get Paged Data

        public PagedList<TEntity> GetPaged<TOrder>(int pageNumber, int pageSize = 10, bool isAscendingOrder = false,
            Expression<Func<TEntity, bool>> predicate = null,
            Expression<Func<TEntity, TOrder>> orderByProperty = null)
        {
            if (pageSize <= 0)
                pageSize = 10;

            var query = predicate == null ? Query() : Query(predicate);

            var rowsCount = query.ToList().Count;

            if (rowsCount <= pageSize || pageNumber < 1)
                pageNumber = 1;

            if (orderByProperty != null)
            {
                query = isAscendingOrder ? query.OrderBy(orderByProperty) : query.OrderByDescending(orderByProperty);
            }

            return PagedList<TEntity>.Create(query, pageNumber, pageSize, rowsCount);
        }

        #endregion Get Paged Data

        #region Get Paged Data Async

        public async Task<PagedList<TEntity>> GetPagedAsync<TOrder>(int pageNumber, int pageSize = 10, bool isAscendingOrder = false,
            Expression<Func<TEntity, bool>> predicate = null,
            Expression<Func<TEntity, TOrder>> orderByProperty = null)
        {
            if (pageSize <= 0)
                pageSize = 10;

            var query = predicate == null ? Query() : Query(predicate);

            var rowsCount = query.ToList().Count;

            if (rowsCount <= pageSize || pageNumber <= 1)
                pageNumber = 1;

            if (orderByProperty != null)
            {
                query = isAscendingOrder ? query.OrderBy(orderByProperty) : query.OrderByDescending(orderByProperty);
            }

            return await PagedList<TEntity>.CreateAsync(query, pageNumber, pageSize, rowsCount);
        }

        #endregion Get Paged Data Async

        #region Get Paged Data

        public async Task<PagedList<TEntity>> GetPagedAsync(int pageNumber, int pageSize, IQueryable<TEntity> query)
        {
            if (pageSize <= 0)
                pageSize = 10;

            var rowsCount = query.ToList().Count;

            if (rowsCount <= pageSize || pageNumber <= 1)
                pageNumber = 1;

            return await PagedList<TEntity>.CreateAsync(query, pageNumber, pageSize, rowsCount);
        }

        public Task AddRangeAsync(IList<TEntity> tEntities)
        {
            return Context.Set<TEntity>().AddRangeAsync(tEntities);
        }

        #endregion Get Paged Data

        #region CRUD

        public void Insert(TEntity entity)
        {
            Context.Set<TEntity>().Add(entity);
        }

        public void Update(TEntity entity)
        {
            if (Context.Set<TEntity>().Local.All(e => e != entity))
                Context.Set<TEntity>().Attach(entity);
            Context.Entry(entity).State = EntityState.Modified;
        }

        public void Delete(TEntity entity)
        {
            Context.Set<TEntity>().Remove(entity);
        }

        public void DeleteRange(Expression<Func<TEntity, bool>> predicate)
        {
            var data = Context.Set<TEntity>().Where(predicate);
            Context.Set<TEntity>().RemoveRange(data);
        }

        public void RemoveRange(IEnumerable<TEntity> entities)
        {
            Context.Set<TEntity>().RemoveRange(entities);
        }

        #endregion CRUD
    }
}
