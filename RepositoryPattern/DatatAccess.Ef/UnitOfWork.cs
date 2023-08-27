using Dapper;
using DatatAccess.Ef.Contracts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatatAccess.Ef
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DbContext _context;
        private readonly IDbConnection _connection;
        private bool _disposed;

        public UnitOfWork(DbContext context, IDbConnection connection)
        {
            _context = context;

            if (context != null) _connection = connection;
        }

        #region IProgramAbilitySupport

        public int ExecuteStoredProcedure(string name, object data, int? commandTimeout = 0)
        {
            return _connection.Execute(name, data, commandType: CommandType.StoredProcedure, commandTimeout: _connection.ConnectionTimeout);
        }

        public async Task<int> ExecuteStoredProcedureAsync(string name, object data, int? commandTimeout = 0)
        {
            try
            {
                return await _connection.ExecuteAsync(name, data, commandType: CommandType.StoredProcedure, commandTimeout: _connection.ConnectionTimeout).ConfigureAwait(false);
            }
            catch (Exception exp)
            {
                throw;
            }
        }

        public TEntity ExecuteStoredProcedureSingle<TEntity>(string name, object data, int? commandTimeout = 0)
        {
            return _connection.QuerySingle<TEntity>(name, data, commandType: CommandType.StoredProcedure,
                    commandTimeout: _connection.ConnectionTimeout);
        }

        public async Task<TEntity> ExecuteStoredProcedureSingleAsync<TEntity>(string name, object data, int? commandTimeout = 0)
        {
            return await _connection.QuerySingleAsync<TEntity>(name, data, commandType: CommandType.StoredProcedure,
                    commandTimeout: _connection.ConnectionTimeout).ConfigureAwait(false);
        }

        public IEnumerable<TEntity> ExecuteStoredProcedure<TEntity>(string name, object data, int? commandTimeout = 0)
        {
            return _connection.Query<TEntity>(name, data, commandType: CommandType.StoredProcedure, commandTimeout: _connection.ConnectionTimeout);
        }

        public async Task<IEnumerable<TEntity>> ExecuteStoredProcedureAsync<TEntity>(string name, object data, int? commandTimeout = 0)
        {
            return await _connection.QueryAsync<TEntity>(name, data, commandType: CommandType.StoredProcedure, commandTimeout: _connection.ConnectionTimeout).ConfigureAwait(false);
        }

        public IEnumerable<TEntity> ExecuteStoredProcedure<TEntity>(string name, int? commandTimeout = 0)
        {
            return _connection.Query<TEntity>(name, commandType: CommandType.StoredProcedure, commandTimeout: _connection.ConnectionTimeout);
        }

        public async Task<IEnumerable<TEntity>> ExecuteStoredProcedureAsync<TEntity>(string name, int? commandTimeout = 0)
        {
            return await _connection.QueryAsync<TEntity>(name, commandType: CommandType.StoredProcedure, commandTimeout: _connection.ConnectionTimeout).ConfigureAwait(false);
        }

        public TEntity ExecuteStoredProcedureSingle<TEntity>(string name, int? commandTimeout = 0)
        {
            return _connection.QuerySingle<TEntity>(name, commandType: CommandType.StoredProcedure, commandTimeout: _connection.ConnectionTimeout);
        }

        public async Task<TEntity> ExecuteStoredProcedureSingleAsync<TEntity>(string name, int? commandTimeout = 0)
        {
            return await _connection.QuerySingleAsync<TEntity>(name, commandType: CommandType.StoredProcedure, commandTimeout: _connection.ConnectionTimeout).ConfigureAwait(false);
        }

        public TEntity ExecuteFunctionSingle<TEntity>(string name, string predicate, object data, int? commandTimeout = 0)
        {
            return _connection.QuerySingle<TEntity>($"SELECT {name}({predicate})", data, commandTimeout: _connection.ConnectionTimeout);
        }

        public async Task<TEntity> ExecuteFunctionSingleAsync<TEntity>(string name, string predicate, object data, int? commandTimeout = 0)
        {
            return await _connection.QuerySingleAsync<TEntity>($"SELECT {name}({predicate})", data, commandTimeout: _connection.ConnectionTimeout).ConfigureAwait(false);
        }

        public IEnumerable<TEntity> ExecuteFunction<TEntity>(string name, string predicate, object data, int? commandTimeout = 0)
        {
            return _connection.Query<TEntity>($"SELECT {name}({predicate})", data, commandTimeout: _connection.ConnectionTimeout);
        }

        public async Task<IEnumerable<TEntity>> ExecuteFunctionAsync<TEntity>(string name, string predicate, object data, int? commandTimeout = 0)
        {
            return await _connection.QueryAsync<TEntity>($"SELECT {name}({predicate})", data, commandTimeout: _connection.ConnectionTimeout).ConfigureAwait(false);
        }

        public async Task<IEnumerable<dynamic>> ExecuteQueryAsync(string query)
        {
            return await _connection.QueryAsync(query, commandTimeout: _connection.ConnectionTimeout).ConfigureAwait(false);
        }

        #endregion IProgramAbilitySupport

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync().ConfigureAwait(false);
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }

        public void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
        }

        public IRepository<TEntity> GetRepository<TEntity>() where TEntity : class
        {
            return new Repository<TEntity>(_context);
        }
    }
}
