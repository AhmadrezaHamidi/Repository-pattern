using Dapper;
using DataAccess.Dapper.Contracts;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Dapper
{
    public class UnitOfWork : IDisposable, IUnitOfWork
    {
        private readonly SqlConnection _connection;

        public UnitOfWork(string connectionString)
        {
            _connection = new(connectionString);
        }

        public IDataService<T> GetDataService<T>() where T : class
        {
            return new DataService<T>(_connection);
        }

        public IRepository<T> GetDataRepository<T>() where T : class, new()
        {
            return new Repository<T>(_connection);
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
            catch (Exception)
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

        public void Dispose()
        {
            _connection.Close();
            _connection.Dispose();
        }
    }
}
