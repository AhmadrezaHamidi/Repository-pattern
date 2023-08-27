using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Dapper.Contracts
{
    public interface IRepository<T> where T : class, new()
    {
        string Schema { get; set; }
        string TableName { get; }

        int? Add(T entity);
        Task<int> AddAsync(T entity);
        bool Any(Expression<Func<T, bool>> predicate);
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
        int Delete(T entity);
        int Delete(T entity, Expression<Func<T, bool>> predicate);
        int DeleteAll();
        Task<int> DeleteAllAsync();
        Task<int> DeleteAsync(T entity);
        Task<int> DeleteAsync(T entity, Expression<Func<T, bool>> predicate);
        IEnumerable<T> Find(bool distinct, Expression<Func<T, bool>> predicate);
        IEnumerable<T> Find(Expression<Func<T, bool>> predicate);
        IEnumerable<T> Find(string columns, bool distinct, Expression<Func<T, bool>> predicate);
        Task<IEnumerable<T>> FindAsync(bool distinct, Expression<Func<T, bool>> predicate);
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        Task<IEnumerable<T>> FindAsync(string columns, bool distinct, Expression<Func<T, bool>> predicate);
        T First();
        T First(Expression<Func<T, bool>> predicate);
        T First(Expression<Func<T, bool>> predicate, string columns);
        T First(string columns);
        Task<T> FirstAsync();
        Task<T> FirstAsync(Expression<Func<T, bool>> predicate);
        Task<T> FirstAsync(Expression<Func<T, bool>> predicate, string columns);
        Task<T> FirstAsync(string columns);
        T FirstOrDefault();
        T FirstOrDefault(Expression<Func<T, bool>> predicate);
        T FirstOrDefault(Expression<Func<T, bool>> predicate, string columns);
        T FirstOrDefault(string columns);
        Task<T> FirstOrDefaultAsync();
        Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);
        Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, string columns);
        Task<T> FirstOrDefaultAsync(string columns);
        IEnumerable<T> GetAll();
        IEnumerable<T> GetAll(string columns);
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> GetAllAsync(string columns);
        void Insert(T entity);
        Task InsertAsync(T entity);
        T Last();
        T Last(Expression<Func<T, bool>> predicate);
        T Last(Expression<Func<T, bool>> predicate, string columns);
        T Last(string columns);
        Task<T> LastAsync();
        Task<T> LastAsync(Expression<Func<T, bool>> predicate);
        Task<T> LastAsync(Expression<Func<T, bool>> predicate, string columns);
        Task<T> LastAsync(string columns);
        T LastOrDefault();
        T LastOrDefault(Expression<Func<T, bool>> predicate);
        T LastOrDefault(Expression<Func<T, bool>> predicate, string columns);
        T LastOrDefault(string columns);
        Task<T> LastOrDefaultAsync();
        Task<T> LastOrDefaultAsync(Expression<Func<T, bool>> predicate);
        Task<T> LastOrDefaultAsync(Expression<Func<T, bool>> predicate, string columns);
        Task<T> LastOrDefaultAsync(string columns);
        T Single(Expression<Func<T, bool>> predicate);
        T Single(Expression<Func<T, bool>> predicate, string columns);
        Task<T> SingleAsync(Expression<Func<T, bool>> predicate);
        Task<T> SingleAsync(Expression<Func<T, bool>> predicate, string columns);
        T SingleOrDefault(Expression<Func<T, bool>> predicate);
        T SingleOrDefault(Expression<Func<T, bool>> predicate, string columns);
        Task<T> SingleOrDefaultAsync(Expression<Func<T, bool>> predicate);
        Task<T> SingleOrDefaultAsync(Expression<Func<T, bool>> predicate, string columns);
        IEnumerable<T> Top(int count);
        IEnumerable<T> Top(int count, Expression<Func<T, bool>> predicate);
        IEnumerable<T> Top(int count, Expression<Func<T, bool>> predicate, string columns);
        IEnumerable<T> Top(int count, string columns);
        Task<IEnumerable<T>> TopAsync(int count);
        Task<IEnumerable<T>> TopAsync(int count, Expression<Func<T, bool>> predicate);
        Task<IEnumerable<T>> TopAsync(int count, Expression<Func<T, bool>> predicate, string columns);
        Task<IEnumerable<T>> TopAsync(int count, string columns);
        int Update(T entity);
        int Update(T entity, Expression<Func<T, bool>> predicate);
        Task<int> UpdateAsync(T entity);
        Task<int> UpdateAsync(T entity, Expression<Func<T, bool>> predicate);
    }

}
