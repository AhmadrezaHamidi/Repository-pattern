using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Dapper.Contracts
{
    public interface IDataService<T> where T : class
    {
        string Schema { get; }

        string TableName { get; }
        void Add(T entity);
        IEnumerable<T> GetAll();

        Task<IEnumerable<T>> GetAllAsync();

        IEnumerable<T> GetAll(string filterColumns);

        Task<IEnumerable<T>> GetAllAsync(string filterColumns);

        T GetEntity(string predicate, object data);

        Task<T> GetEntityAsync(string predicate, object data);

        T GetEntity(string predicate, object data, string filterColumns);

        Task<T> GetEntityAsync(string predicate, object data, string filterColumns);

        bool HasEntity(string predicate, object data);

        Task<bool> HasEntityAsync(string predicate, object data);

        bool HasEntity(string predicate, object data, string filterColumns);

        Task<bool> HasEntityAsync(string predicate, object data, string filterColumns);

        IEnumerable<T> Find(string predicate, object data, bool distinct);

        Task<IEnumerable<T>> FindAsync(string predicate, object data, bool distinct);

        IEnumerable<T> Find(string predicate, object data, string filterColumns, bool distinct);

        Task<IEnumerable<T>> FindAsync(string predicate, object data, string filterColumns, bool distinct);

        IEnumerable<T> FindTop(int count, string predicate, object data, string filterColumns = "*", bool distinct = false);

        Task<IEnumerable<T>> FindTopAsync(int count, string predicate, object data, string filterColumns = "*", bool distinct = false);

        T First();

        Task<T> FirstAsync();

        T First(string filterColumns);

        Task<T> FirstAsync(string filterColumns);

        T First(string predicate, object data);

        Task<T> FirstAsync(string predicate, object data);

        T First(string predicate, object data, string filterColumns);

        Task<T> FirstAsync(string predicate, object data, string filterColumns);

        T FirstOrDefault();

        Task<T> FirstOrDefaultAsync();

        T FirstOrDefault(string filterColumns);

        Task<T> FirstOrDefaultAsync(string filterColumns);

        T FirstOrDefault(string predicate, object data);

        Task<T> FirstOrDefaultAsync(string predicate, object data);

        T FirstOrDefault(string predicate, object data, string filterColumns);

        Task<T> FirstOrDefaultAsync(string predicate, object data, string filterColumns);

        IEnumerable<T> GetLast(int count);

        Task<IEnumerable<T>> GetLastAsync(int count);

        IEnumerable<T> GetLast(int count, string filterColumns);

        Task<IEnumerable<T>> GetLastAsync(int count, string filterColumns);

        IEnumerable<T> GetLast(int count, string predicate, object data);

        Task<IEnumerable<T>> GetLastAsync(int count, string predicate, object data);

        IEnumerable<T> GetLast(int count, string predicate, object data, string filterColumns);

        Task<IEnumerable<T>> GetLastAsync(int count, string predicate, object data, string filterColumns);

        T GetLastEntity(string predicate, object data);

        Task<T> GetLastEntityAsync(string predicate, object data);

        T GetLastEntity(string predicate, object data, string filterColumns);

        Task<T> GetLastEntityAsync(string predicate, object data, string filterColumns);

        IEnumerable<T> GetTop(int count);

        Task<IEnumerable<T>> GetTopAsync(int count);

        IEnumerable<T> GetTop(int count, string filterColumns);

        Task<IEnumerable<T>> GetTopAsync(int count, string filterColumns);

        IEnumerable<T> GetTop(int count, string predicate, object data);

        Task<IEnumerable<T>> GetTopAsync(int count, string predicate, object data);

        IEnumerable<T> GetTop(int count, string predicate, object data, string filterColumns);

        Task<IEnumerable<T>> GetTopAsync(int count, string predicate, object data, string filterColumns);

        T Last();

        Task<T> LastAsync();

        T Last(string filterColumns);

        Task<T> LastAsync(string filterColumns);

        T Last(string predicate, object data);

        Task<T> LastAsync(string predicate, object data);

        T Last(string predicate, object data, string filterColumns);

        Task<T> LastAsync(string predicate, object data, string filterColumns);

        T LastOrDefault();

        Task<T> LastOrDefaultAsync();

        T LastOrDefault(string filterColumns);

        Task<T> LastOrDefaultAsync(string filterColumns);

        T LastOrDefault(string predicate, object data);

        Task<T> LastOrDefaultAsync(string predicate, object data);

        T LastOrDefault(string predicate, object data, string filterColumns);
        Task<T> LastOrDefaultAsync(string predicate, object data, string filterColumns);

        T Single(string predicate, object data);

        Task<T> SingleAsync(string predicate, object data);

        T Single(string predicate, object data, string filterColumns);

        Task<T> SingleAsync(string predicate, object data, string filterColumns);

        T SingleOrDefault();

        Task<T> SingleOrDefaultAsync();

        T SingleOrDefault(string filterColumns);

        Task<T> SingleOrDefaultAsync(string filterColumns);

        T SingleOrDefault(string predicate, object data);

        Task<T> SingleOrDefaultAsync(string predicate, object data);

        T SingleOrDefault(string predicate, object data, string filterColumns);

        Task<T> SingleOrDefaultAsync(string predicate, object data, string filterColumns);

        PagedData<T> GetPagedData(int pageIndex);

        Task<PagedData<T>> GetPagedDataAsync(int pageIndex);

        PagedData<T> GetPagedData(int pageIndex, string filterColumns);

        Task<PagedData<T>> GetPagedDataAsync(int pageIndex, string filterColumns);

        PagedData<T> GetPagedData(int pageSize, int pageIndex);

        Task<PagedData<T>> GetPagedDataAsync(int pageSize, int pageIndex);

        PagedData<T> GetPagedData(int pageSize, int pageIndex, string filterColumns);

        Task<PagedData<T>> GetPagedDataAsync(int pageSize, int pageIndex, string filterColumns);

        PagedData<T> GetPagedData(int pageSize, int pageIndex, string predicate, object data);

        Task<PagedData<T>> GetPagedDataAsync(int pageSize, int pageIndex, string predicate, object data);

        PagedData<T> GetPagedData(int pageSize, int pageIndex, string predicate, string filterColumns, object data);

        Task<PagedData<T>> GetPagedDataAsync(int pageSize, int pageIndex, string predicate, string filterColumns, object data);

        PagedData<T> GetPagedData(int pageSize, int pageIndex, string predicate, object data, string orderBy);

        Task<PagedData<T>> GetPagedDataAsync(int pageSize, int pageIndex, string predicate, object data, string orderBy);

        PagedData<T> GetPagedData(int pageSize, int pageIndex, string predicate, object data, string orderBy, string filterColumns);

        Task<PagedData<T>> GetPagedDataAsync(int pageSize, int pageIndex, string predicate, object data, string orderBy, string filterColumns);

        PagedData<T> GetPagedData(int pageIndex, int pageSize, string predicate, object data, bool isAscendingOrder, string orderBy);

        Task<PagedData<T>> GetPagedDataAsync(int pageIndex, int pageSize, string predicate, object data, bool isAscendingOrder, string orderBy);

        PagedData<T> GetPagedData(int pageIndex, int pageSize, string predicate, object data, bool isAscendingOrder, string orderBy, string filterColumns);

        Task<PagedData<T>> GetPagedDataAsync(int pageIndex, int pageSize, string predicate, object data, bool isAscendingOrder, string orderBy, string filterColumns);

        TEntity ExecuteStoredProcedureSingle<TEntity>(string name, object data);

        Task<TEntity> ExecuteStoredProcedureSingleAsync<TEntity>(string name, object data);

        TEntity ExecuteStoredProcedureSingle<TEntity>(string name);

        Task<TEntity> ExecuteStoredProcedureSingleAsync<TEntity>(string name);

        IEnumerable<TEntity> ExecuteStoredProcedure<TEntity>(string name, object data);

        Task<IEnumerable<TEntity>> ExecuteStoredProcedureAsync<TEntity>(string name, object data);

        IEnumerable<TEntity> ExecuteStoredProcedure<TEntity>(string name);

        Task<IEnumerable<TEntity>> ExecuteStoredProcedureAsync<TEntity>(string name);

        TEntity ExecuteFunctionSingle<TEntity>(string name, string predicate, object data);

        Task<TEntity> ExecuteFunctionSingleAsync<TEntity>(string name, string predicate, object data);

        IEnumerable<TEntity> ExecuteFunction<TEntity>(string name, string predicate, object data);

        Task<IEnumerable<TEntity>> ExecuteFunctionAsync<TEntity>(string name, string predicate, object data);

        #region CRUD

        int Insert(T entity, string columns, string columnsData);

        Task<int> InsertAsync(T entity, string columns, string columnsData);

        bool Insert(string columns, string columnsData, T entity);

        Task<bool> InsertAsync(string columns, string columnsData, T entity);

        bool Insert(T entity);

        Task<bool> InsertAsync(T entity);

        int Insert(T entity, bool auto);

        Task<int> InsertAsync(T entity, bool auto);

        bool Update(T entity, string columns, string whereColumns);

        Task<bool> UpdateAsync(T entity, string columns, string whereColumns);

        int Update(object entity, Expression<Func<T, bool>> predicate);

        Task<int> UpdateAsync(object entity, Expression<Func<T, bool>> predicate);

        bool Delete(T entity, string whereColumns);

        Task<bool> DeleteAsync(T entity, string whereColumns);

        int Delete(T entity);

        Task<int> DeleteAsync(T entity);

        int DeleteAll();

        Task<int> DeleteAllAsync();

        #endregion CRUD
    }

}
