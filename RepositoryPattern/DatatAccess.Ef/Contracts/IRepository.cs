using DatatAccess.Ef.Common;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DatatAccess.Ef.Contracts
{
    public interface IRepository<TEntity> where TEntity : class
    {
        IQueryable<TEntity> Query(Expression<Func<TEntity, bool>> predicate = null);

        IEnumerable<TEntity> GetAll();

        IQueryable<TEntity> GetAllData();

        Task<IEnumerable<TEntity>> GetAllAsync();

        bool HasEntity(Expression<Func<TEntity, bool>> predicate);

        Task<bool> HasEntityAsync(Expression<Func<TEntity, bool>> predicate);

        TEntity GetEntity(Expression<Func<TEntity, bool>> predicate);

        Task<TEntity> GetEntityAsync(Expression<Func<TEntity, bool>> predicate);

        TEntity GetLastEntity(Expression<Func<TEntity, bool>> predicate);

        Task<TEntity> GetLastEntityAsync(Expression<Func<TEntity, bool>> predicate);

        TEntity FirstOrDefault();

        Task<TEntity> FirstOrDefaultAsync();

        TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate);

        Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate);

        TEntity First();

        Task<TEntity> FirstAsync();

        TEntity First(Expression<Func<TEntity, bool>> predicate);

        Task<TEntity> FirstAsync(Expression<Func<TEntity, bool>> predicate);

        Task<EntityEntry<TEntity>> InsertAsync(TEntity entity);

        TEntity Last();

        Task<TEntity> LastAsync();

        TEntity Last(Expression<Func<TEntity, bool>> predicate);

        Task<TEntity> LastAsync(Expression<Func<TEntity, bool>> predicate);

        TEntity LastOrDefault();

        Task<TEntity> LastOrDefaultAsync();

        TEntity LastOrDefault(Expression<Func<TEntity, bool>> predicate);

        Task<TEntity> LastOrDefaultAsync(Expression<Func<TEntity, bool>> predicate);

        TEntity SingleOrDefault();

        Task<TEntity> SingleOrDefaultAsync();

        TEntity SingleOrDefault(Expression<Func<TEntity, bool>> predicate);

        Task<TEntity> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate);

        TEntity Single();

        Task AddRangeAsync(List<TEntity> entities);

        Task<TEntity> SingleAsync();

        TEntity Single(Expression<Func<TEntity, bool>> predicate);

        Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>> predicate);

        IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate);

        Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate);

        IEnumerable<TEntity> GetTop(int count);

        Task<IEnumerable<TEntity>> GetTopAsync(int count);

        IEnumerable<TEntity> GetTop(int count, Expression<Func<TEntity, bool>> predicate);

        Task<IEnumerable<TEntity>> GetTopAsync(int count, Expression<Func<TEntity, bool>> predicate);

        IEnumerable<TEntity> GetLast(int count);

        Task<IEnumerable<TEntity>> GetLastAsync(int count);

        IEnumerable<TEntity> GetLast(int count, Expression<Func<TEntity, bool>> predicate);

        Task<IEnumerable<TEntity>> GetLastAsync(int count, Expression<Func<TEntity, bool>> predicate);

        PagedList<TEntity> GetPaged<TOrder>(int pageNumber, int pageSize = 10, bool isAscendingOrder = false,
            Expression<Func<TEntity, bool>> predicate = null,
            Expression<Func<TEntity, TOrder>> orderByProperty = null);

        Task<PagedList<TEntity>> GetPagedAsync<TOrder>(int pageNumber, int pageSize = 10, bool isAscendingOrder = false,
            Expression<Func<TEntity, bool>> predicate = null,
            Expression<Func<TEntity, TOrder>> orderByProperty = null);

        Task<PagedList<TEntity>> GetPagedAsync(int pageNumber, int pageSize, IQueryable<TEntity> query);

        Task AddRangeAsync(IList<TEntity> tEntities);

        void Insert(TEntity entity);

        void Update(TEntity entity);

        void Delete(TEntity entity);

        void DeleteRange(Expression<Func<TEntity, bool>> predicate);

        void RemoveRange(IEnumerable<TEntity> entities);
    }
}
