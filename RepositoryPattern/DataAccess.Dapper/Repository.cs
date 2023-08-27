using DataAccess.Dapper.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Dapper.FastCrud;
using Core.Extensions;
using Core.Domain;

namespace DataAccess.Dapper
{
    public class Repository<T> : IRepository<T> where T : class, new()
    {
        private readonly IDbConnection _connection;
        private string _tableName;
        private string _rowTableName;
        private string _allColumns = "*";
        private string _schema = "dbo";

        public Repository(IDbConnection connection)
        {
            _connection = connection;
            _connection = connection;

            //Get name of table from object type
            var tblName = GetTableName<T>();
            var temp = tblName.Split('.');

            if (temp.Length > 1)
            {
                _rowTableName = temp[temp.Length - 1];
                _tableName = $"[{_schema}].[{_rowTableName}]";
            }
            else
            {
                var shcema = GetSchema<T>();
                if (!string.IsNullOrEmpty(shcema))
                    _schema = shcema;

                _rowTableName = tblName;
                _tableName = $"[{_schema}].[{_rowTableName}]";
            }

            _allColumns = GetAllColumns<T>();
        }

        private string GetTableName<Tbl>()
        {
            if (typeof(Tbl).GetCustomAttributes(typeof(TableAttribute), true)
                .SingleOrDefault() is TableAttribute table)
            {
                return table.Name;
            }

            var typeName = typeof(Tbl).ToString();
            var i = typeName.IndexOf('`');
            if (i >= 0)
            {
                typeName = typeName[..i];
            }
            return typeName;
        }

        private string GetSchema<Tbl>()
        {
            if (typeof(Tbl).GetCustomAttributes(typeof(TableAttribute), true)
                .SingleOrDefault() is TableAttribute table)
            {
                if (table.Schema != null)
                    return table.Schema;
                else
                    return string.Empty;
            }

            return string.Empty;
        }

        private string GetAllColumns<Tbl>()
        {
            var columns = new List<string>();
            var type = typeof(Tbl);
            var properties = type.GetProperties().ToList();

            foreach (var propertie in properties)
            {
                var columnAttribute = propertie.GetCustomAttribute<ColumnAttribute>();

                if (columnAttribute != null)
                {
                    if (columnAttribute.Name != null)
                        columns.Add($"{columnAttribute.Name} AS {propertie.Name}");
                    else
                        columns.Add(propertie.Name);
                }
                else
                    columns.Add(propertie.Name);
            }

            var result = string.Join(", ", columns.ToArray());
            return result;
        }

        private string GetKeyColumn<Tbl>()
        {
            var column = string.Empty;
            var type = typeof(Tbl);
            var properties = type.GetProperties().ToList();

            foreach (var propertie in properties)
            {
                var keyAttribute = propertie.GetCustomAttribute<KeyAttribute>();

                if (keyAttribute != null)
                {
                    column = propertie.Name;
                    break;
                }
                else
                {
                    if (propertie.Name.ToLower() == "id" || propertie.Name.ToLower() == $"{_rowTableName}id")
                    {
                        column = propertie.Name;
                        break;
                    }
                }
            }

            return column;
        }

        private (string Name, object? Value) GetKey(T entity)
        {
            var type = entity.GetType();
            var keyColumn = GetKeyColumn<T>();
            var property = type.GetProperty(keyColumn);
            return (keyColumn, property?.GetValue(entity));
        }

        private (string sql, DynamicParameters param) GetWhere(Expression<Func<T, bool>> expression, string sql)
        {
            var whereSql = expression?.ToSql() ?? WherePart.Empty;
            var parameter = new DynamicParameters();

            foreach (var param in whereSql.Parameters)
            {
                parameter.Add(param.Key, param.Value, param.Type);
            }

            sql = sql.Replace("{where}", whereSql.HasSql ? whereSql.Sql : string.Empty);

            return (sql, parameter);
        }

        private IEnumerable<string> GetKeys(Type entityType, string tableName)
        {
            var properties = entityType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                     .Where(x => entityType.Name.Contains("AnonymousType") ? x.CanRead : x.CanWrite && x.CanRead)
                     .ToList();

            var finalprops = new List<string>();

            foreach (var item in properties)
            {
                var prop = TypeDescriptor.GetProperties(entityType).OfType<PropertyDescriptor>().FirstOrDefault(x => x.Name == item.Name);
                var isKey = prop?.Attributes.OfType<KeyAttribute>().Any();

                isKey ??= false;

                if (item.Name.ToLower() == "id" || string.Equals(item.Name, $"{tableName}id", StringComparison.CurrentCultureIgnoreCase) || isKey.Value)
                {
                    finalprops.Add(item.Name);
                }
            }

            return finalprops;
        }

        private IEnumerable<string> GetPropertiesNames(Type entityType, string tableName)
        {
            var properties = entityType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                 .Where(x => entityType.Name.Contains("AnonymousType") ? x.CanRead : x.CanWrite && x.CanRead)
                                 .ToList();

            var finalprops = new List<string>();
            foreach (var item in properties)
            {
                if (item.Name.ToLower() == "id" || string.Equals(item.Name, $"{tableName}id", StringComparison.CurrentCultureIgnoreCase)) continue;

                var prop = TypeDescriptor.GetProperties(entityType).OfType<PropertyDescriptor>().FirstOrDefault(x => x.Name == item.Name);
                var notMapped = prop?.Attributes.OfType<NotMappedAttribute>().Any();
                var databaseGenerated = prop?.Attributes.OfType<DatabaseGeneratedAttribute>().Any();

                if (!(notMapped == null || notMapped.Value || databaseGenerated == null || databaseGenerated.Value))
                {
                    // Only add properties names which do not have NotMappedAttribute Set or DatabaseGeneratedAttribute

                    //var columnAttribute = item.GetCustomAttribute<ColumnAttribute>();

                    //if (columnAttribute != null)
                    //{
                    //    if (columnAttribute.Name != null)
                    //        finalprops.Add($"{columnAttribute.Name} AS {item.Name}");
                    //    else
                    //        finalprops.Add(item.Name);
                    //}
                    //else
                    finalprops.Add(item.Name);
                }
            }

            return finalprops;
        }

        private DynamicParameters GetProperties(T entity)
        {
            var type = entity.GetType();
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                     .Where(x => type.Name.Contains("AnonymousType") ? x.CanRead : x.CanWrite && x.CanRead)
                     .ToList();

            DynamicParameters finalProps = new();

            foreach (var property in properties)
            {
                var isKeyAttribute = property.GetCustomAttribute<KeyAttribute>();
                var isNotMapped = property.GetCustomAttribute<NotMappedAttribute>();

                if (isKeyAttribute != null || isNotMapped != null ||
                    property.Name.ToLower() == "id" || property.Name.ToLower() == $"{_rowTableName}id")
                {
                    continue;
                }

                var name = string.Empty;

                var columnAttribute = property.GetCustomAttribute<ColumnAttribute>();
                if (columnAttribute != null && !string.IsNullOrEmpty(columnAttribute.Name))
                {
                    name = columnAttribute.Name;
                }
                else
                {
                    name = property.Name;
                }

                var value = property.GetValue(entity);

                if (value != null && !string.IsNullOrEmpty(value?.ToString()))
                {
                    finalProps.Add(name, value);
                }
            }

            return finalProps;
        }

        public string Schema
        {
            get => _schema;
            set
            {
                _schema = $"{value.Replace("[", "").Replace("]", "")}";
                _tableName = $"[{_schema}].{_rowTableName}";
            }
        }

        public string TableName { get; } = "";

        #region Find

        public IEnumerable<T> Find(Expression<Func<T, bool>> predicate)
        {
            return Find(_allColumns, false, predicate);
        }

        public IEnumerable<T> Find(bool distinct, Expression<Func<T, bool>> predicate)
        {
            return Find(_allColumns, distinct, predicate);
        }

        public IEnumerable<T> Find(string columns, bool distinct, Expression<Func<T, bool>> predicate)
        {
            var d = distinct ? "distinct" : "";
            var sql = $"SELECT {d} {columns} FROM {_tableName} WHERE {{where}}";
            var whereSql = GetWhere(predicate, sql);

            var result = _connection.Query<T>(whereSql.sql, whereSql.param);
            return result;
        }

        public Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return FindAsync(_allColumns, false, predicate);
        }

        public Task<IEnumerable<T>> FindAsync(bool distinct, Expression<Func<T, bool>> predicate)
        {
            return FindAsync(_allColumns, distinct, predicate);
        }

        public Task<IEnumerable<T>> FindAsync(string columns, bool distinct, Expression<Func<T, bool>> predicate)
        {
            var d = distinct ? "distinct" : "";
            var sql = $"SELECT {d} {columns} FROM {_tableName} WHERE {{where}}";
            var whereSql = GetWhere(predicate, sql);

            var result = _connection.QueryAsync<T>(whereSql.sql, whereSql.param);
            return result;
        }

        #endregion Find

        #region First

        public T First()
        {
            return First(_allColumns);
        }

        public T First(string columns)
        {
            T result = _connection.QueryFirst<T>($"SELECT {columns} FROM {_tableName}");
            return result;
        }

        public T First(Expression<Func<T, bool>> predicate)
        {
            return First(predicate, _allColumns);
        }

        public T First(Expression<Func<T, bool>> predicate, string columns)
        {
            var sql = $"SELECT {columns} FROM {_tableName} WHERE {{where}}";
            var whereSql = GetWhere(predicate, sql);

            return _connection.QueryFirst<T>(whereSql.sql, whereSql.param);
        }

        public Task<T> FirstAsync()
        {
            return FirstAsync(_allColumns);
        }

        public Task<T> FirstAsync(string columns)
        {
            return _connection.QueryFirstAsync<T>($"SELECT {columns} FROM {_tableName}");
        }

        public Task<T> FirstAsync(Expression<Func<T, bool>> predicate)
        {
            return FirstAsync(predicate, _allColumns);
        }

        public Task<T> FirstAsync(Expression<Func<T, bool>> predicate, string columns)
        {
            var sql = $"SELECT {columns} FROM {_tableName} WHERE {{where}}";
            var whereSql = GetWhere(predicate, sql);

            return _connection.QueryFirstAsync<T>(whereSql.sql, whereSql.param);
        }

        #endregion First

        #region FirstOrDefault

        public T FirstOrDefault()
        {
            return FirstOrDefault(_allColumns);
        }

        public T FirstOrDefault(string columns)
        {
            return _connection.QueryFirstOrDefault<T>($"SELECT {columns} FROM {_tableName}");
        }

        public T FirstOrDefault(Expression<Func<T, bool>> predicate)
        {
            return FirstOrDefault(predicate, _allColumns);
        }

        public T FirstOrDefault(Expression<Func<T, bool>> predicate, string columns)
        {
            var sql = $"SELECT {columns} FROM {_tableName} WHERE {{where}}";
            var whereSql = GetWhere(predicate, sql);

            var result = _connection.QueryFirstOrDefault<T>(whereSql.sql, whereSql.param);
            return result;
        }

        public Task<T> FirstOrDefaultAsync()
        {
            return FirstOrDefaultAsync(_allColumns);
        }

        public Task<T> FirstOrDefaultAsync(string columns)
        {
            return _connection.QueryFirstOrDefaultAsync<T>($"SELECT {columns} FROM {_tableName}");
        }

        public Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
        {
            return FirstOrDefaultAsync(predicate, _allColumns);
        }

        public Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, string columns)
        {
            var sql = $"SELECT {columns} FROM {_tableName} WHERE {{where}}";
            var whereSql = GetWhere(predicate, sql);

            return _connection.QueryFirstOrDefaultAsync<T>(whereSql.sql, whereSql.param);
        }

        #endregion FirstOrDefault

        #region GetAll

        public IEnumerable<T> GetAll()
        {
            return GetAll(_allColumns);
        }

        public IEnumerable<T> GetAll(string columns)
        {
            var result = _connection.Query<T>($"SELECT {columns} FROM {_tableName} ");
            return result;
        }

        public Task<IEnumerable<T>> GetAllAsync()
        {
            return GetAllAsync(_allColumns);
        }

        public Task<IEnumerable<T>> GetAllAsync(string columns)
        {
            var result = _connection.QueryAsync<T>($"SELECT {columns} FROM {_tableName} ");
            return result;
        }

        #endregion GetAll

        #region Single

        public T Single(Expression<Func<T, bool>> predicate)
        {
            return Single(predicate, _allColumns);
        }

        public T Single(Expression<Func<T, bool>> predicate, string columns)
        {
            var sql = $"SELECT {columns} FROM {_tableName} WHERE {{where}}";
            var whereSql = GetWhere(predicate, sql);
            return _connection.QuerySingle<T>(whereSql.sql, whereSql.param);
        }

        public Task<T> SingleAsync(Expression<Func<T, bool>> predicate)
        {
            return SingleAsync(predicate, _allColumns);
        }

        public Task<T> SingleAsync(Expression<Func<T, bool>> predicate, string columns)
        {
            var sql = $"SELECT {columns} FROM {_tableName} WHERE {{where}}";
            var whereSql = GetWhere(predicate, sql);
            return _connection.QuerySingleAsync<T>(whereSql.sql, whereSql.param);
        }

        #endregion Single

        #region SingleOrDefault

        public T SingleOrDefault(Expression<Func<T, bool>> predicate)
        {
            return SingleOrDefault(predicate, _allColumns);
        }

        public T SingleOrDefault(Expression<Func<T, bool>> predicate, string columns)
        {
            var sql = $"SELECT {columns} FROM {_tableName} WHERE {{where}}";
            var whereSql = GetWhere(predicate, sql);
            return _connection.QuerySingleOrDefault<T>(whereSql.sql, whereSql.param);
        }

        public Task<T> SingleOrDefaultAsync(Expression<Func<T, bool>> predicate)
        {
            return SingleOrDefaultAsync(predicate, _allColumns);
        }

        public Task<T> SingleOrDefaultAsync(Expression<Func<T, bool>> predicate, string columns)
        {
            var sql = $"SELECT {columns} FROM {_tableName} WHERE {{where}}";
            var whereSql = GetWhere(predicate, sql);
            return _connection.QuerySingleOrDefaultAsync<T>(whereSql.sql, whereSql.param);
        }

        #endregion SingleOrDefault

        #region Top

        public IEnumerable<T> Top(int count)
        {
            return Top(count, _allColumns);
        }

        public IEnumerable<T> Top(int count, string columns)
        {
            return _connection.Query<T>($"SELECT TOP {count} {columns} FROM {_tableName} ");
        }

        public IEnumerable<T> Top(int count, Expression<Func<T, bool>> predicate)
        {
            return Top(count, predicate, _allColumns);
        }

        public IEnumerable<T> Top(int count, Expression<Func<T, bool>> predicate, string columns)
        {
            var sql = $"SELECT TOP {count} {columns} FROM {_tableName} WHERE {{where}}";
            var whereSql = GetWhere(predicate, sql);
            return _connection.Query<T>(whereSql.sql, whereSql.param);
        }

        public Task<IEnumerable<T>> TopAsync(int count)
        {
            return TopAsync(count, _allColumns);
        }

        public Task<IEnumerable<T>> TopAsync(int count, string columns)
        {
            return _connection.QueryAsync<T>($"SELECT TOP {count} {columns} FROM {_tableName} ");
        }

        public Task<IEnumerable<T>> TopAsync(int count, Expression<Func<T, bool>> predicate)
        {
            return TopAsync(count, predicate, _allColumns);
        }

        public Task<IEnumerable<T>> TopAsync(int count, Expression<Func<T, bool>> predicate, string columns)
        {
            var sql = $"SELECT TOP {count} {columns} FROM {_tableName} WHERE {{where}}";
            var whereSql = GetWhere(predicate, sql);
            return _connection.QueryAsync<T>(whereSql.sql, whereSql.param);
        }

        #endregion Top

        #region Any

        public bool Any(Expression<Func<T, bool>> predicate)
        {
            var sql = $"SELECT {_allColumns} FROM {_tableName} WHERE {{where}}";
            var whereSql = GetWhere(predicate, sql);

            return _connection.QueryFirstOrDefault<T>(whereSql.sql, whereSql.param) != null;
        }

        public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
        {
            var sql = $"SELECT {_allColumns} FROM {_tableName} WHERE {{where}}";
            var whereSql = GetWhere(predicate, sql);

            return (await _connection.QueryFirstOrDefaultAsync<T>(whereSql.sql, whereSql.param)) != null;
        }

        #endregion Any

        #region Last

        public T Last()
        {
            return Last(_allColumns);
        }

        public T Last(string columns)
        {
            var SortType = "Desc";
            var orderBy = GetKeyColumn<T>();

            return _connection.QueryFirst<T>($"SELECT {columns} FROM {_tableName} Order By {orderBy} {SortType}");
        }

        public T Last(Expression<Func<T, bool>> predicate)
        {
            return Last(predicate, _allColumns);
        }

        public T Last(Expression<Func<T, bool>> predicate, string columns)
        {
            var SortType = "Desc";
            var orderBy = $"{_tableName}Id";
            var sql = $"SELECT {columns} FROM {_tableName} WHERE {{where}} Order By {orderBy} {SortType}";
            var whereSql = GetWhere(predicate, sql);
            return _connection.QueryFirst<T>(whereSql.sql, whereSql.param);
        }

        public Task<T> LastAsync()
        {
            return LastAsync(_allColumns);
        }

        public Task<T> LastAsync(string columns)
        {
            var SortType = "Desc";
            var orderBy = GetKeyColumn<T>();

            return _connection.QueryFirstAsync<T>($"SELECT {columns} FROM {_tableName} Order By {orderBy} {SortType}");
        }

        public Task<T> LastAsync(Expression<Func<T, bool>> predicate)
        {
            return LastAsync(predicate, _allColumns);
        }

        public Task<T> LastAsync(Expression<Func<T, bool>> predicate, string columns)
        {
            var SortType = "Desc";
            var orderBy = GetKeyColumn<T>();
            var sql = $"SELECT {columns} FROM {_tableName} WHERE {{where}} Order By {orderBy} {SortType}";
            var whereSql = GetWhere(predicate, sql);
            return _connection.QueryFirstAsync<T>(whereSql.sql, whereSql.param);
        }

        #endregion Last

        #region LastOrDefault

        public T LastOrDefault()
        {
            return LastOrDefault(_allColumns);
        }

        public T LastOrDefault(string columns)
        {
            var SortType = "Desc";
            var orderBy = GetKeyColumn<T>();

            return _connection.QueryFirstOrDefault<T>($"SELECT {columns} FROM {_tableName} Order By {orderBy} {SortType}");
        }

        public T LastOrDefault(Expression<Func<T, bool>> predicate)
        {
            return LastOrDefault(predicate, _allColumns);
        }

        public T LastOrDefault(Expression<Func<T, bool>> predicate, string columns)
        {
            var SortType = "Desc";
            var orderBy = GetKeyColumn<T>();

            var sql = $"SELECT {columns} FROM {_tableName} WHERE {{where}} Order By {orderBy} {SortType}";
            var whereSql = GetWhere(predicate, sql);
            return _connection.QueryFirstOrDefault<T>(whereSql.sql, whereSql.param);
        }

        public Task<T> LastOrDefaultAsync()
        {
            return LastOrDefaultAsync(_allColumns);
        }

        public Task<T> LastOrDefaultAsync(string columns)
        {
            var SortType = "Desc";
            var orderBy = GetKeyColumn<T>();

            return _connection.QueryFirstOrDefaultAsync<T>($"SELECT {columns} FROM {_tableName} Order By {orderBy} {SortType}");
        }

        public Task<T> LastOrDefaultAsync(Expression<Func<T, bool>> predicate)
        {
            return LastOrDefaultAsync(predicate, _allColumns);
        }

        public Task<T> LastOrDefaultAsync(Expression<Func<T, bool>> predicate, string columns)
        {
            var SortType = "Desc";
            var orderBy = GetKeyColumn<T>();

            var sql = $"SELECT {columns} FROM {_tableName} WHERE {{where}} Order By {orderBy} {SortType}";
            var whereSql = GetWhere(predicate, sql);
            return _connection.QueryFirstOrDefaultAsync<T>(whereSql.sql, whereSql.param);
        }

        #endregion LastOrDefault

        #region Insert

        public void Insert(T entity)
        {
            _connection.Insert(entity);
        }

        public Task InsertAsync(T entity)
        {
            return _connection.InsertAsync(entity);
        }

        public int? Add(T entity)
        {
            _connection.Insert(entity);
            var result = string.Format("SELECT top 1 IDENT_CURRENT('" + _rowTableName + "') FROM " + _tableName);
            return _connection.QuerySingleOrDefault<int?>(result);
        }

        public Task<int> AddAsync(T entity)
        {
            _connection.InsertAsync(entity);
            var result = string.Format("SELECT top 1 IDENT_CURRENT('" + _tableName + "') FROM " + _tableName);
            return _connection.QuerySingleAsync<int>(result);
        }

        #endregion Insert

        #region Update

        public int Update(T entity)
        {
            var dataType = entity.GetType();
            var properties = GetPropertiesNames(dataType, _rowTableName);
            var columns = string.Empty;
            var (Name, Value) = GetKey(entity);

            foreach (var item in properties)
            {
                columns += $"{item} = @{item},";
            }
            columns = columns.TrimEnd(',');

            var where = string.Empty;
            if (!string.IsNullOrEmpty(Value?.ToString()))
            {
                where = $"WHERE {Name} = {Value}";
            }

            var query = $"Update {_tableName} Set {columns} {where}";
            return _connection.Execute(query, entity);
        }

        public int Update(T entity, Expression<Func<T, bool>> predicate)
        {
            var properties = GetProperties(entity);

            var columns = string.Empty;

            foreach (var item in properties.ParameterNames)
            {
                columns += $"{item} = @{item},";
            }

            columns = columns.TrimEnd(',');
            var query = $"Update {_tableName} Set {columns} WHERE {{where}}";
            var whereSql = GetWhere(predicate, query);
            var sql = whereSql.sql;
            return _connection.Execute(sql, entity);
        }

        public Task<int> UpdateAsync(T entity)
        {
            var dataType = entity.GetType();
            var properties = GetPropertiesNames(dataType, _rowTableName);
            var columns = string.Empty;
            var (Name, Value) = GetKey(entity);

            foreach (var item in properties)
            {
                columns += $"{item} = @{item},";
            }
            columns = columns.TrimEnd(',');
            var where = string.Empty;

            if (!string.IsNullOrEmpty(Value.ToString()))
            {
                where = $"WHERE {Name} = {Value}";
            }

            var query = $"Update {_tableName} Set {columns} {where}";
            return _connection.ExecuteAsync(query, entity);
        }

        public Task<int> UpdateAsync(T entity, Expression<Func<T, bool>> predicate)
        {
            var properties = GetProperties(entity);

            var columns = string.Empty;

            foreach (var item in properties.ParameterNames)
            {
                columns += $"{item} = @{item},";
            }

            columns = columns.TrimEnd(',');
            var query = $"Update {_tableName} Set {columns} WHERE {{where}}";
            var whereSql = GetWhere(predicate, query);
            var sql = whereSql.sql;
            return _connection.ExecuteAsync(sql, entity);
        }

        #endregion Update

        #region Delete

        public int Delete(T entity)
        {
            var (Name, Value) = GetKey(entity);

            var where = string.Empty;
            if (!string.IsNullOrEmpty(Value?.ToString()))
            {
                where = $"WHERE {Name} = {Value}";
            }

            var query = $"Delete From {_tableName} {where}";
            try
            {
                return _connection.Execute(query, entity);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public int Delete(T entity, Expression<Func<T, bool>> predicate)
        {
            try
            {
                var query = $"Delete From {_tableName} Where {{where}}";
                var (sql, _) = GetWhere(predicate, query);
                return _connection.Execute(sql, entity);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public int DeleteAll()
        {
            var result = _connection.Count<T>();
            _connection.Execute($"TRUNCATE TABLE {_tableName}");
            return result;
        }

        public async Task<int> DeleteAsync(T entity)
        {
            var (Name, Value) = GetKey(entity);

            var where = string.Empty;
            if (!string.IsNullOrEmpty(Value?.ToString()))
            {
                where = $"WHERE {Name} = {Value}";
            }

            var query = $"Delete From {_tableName} {where}";
            try
            {
                return await _connection.ExecuteAsync(query, entity);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public async Task<int> DeleteAsync(T entity, Expression<Func<T, bool>> predicate)
        {
            try
            {
                var query = $"Delete From {_tableName} Where {{where}}";
                var (sql, _) = GetWhere(predicate, query);
                return await _connection.ExecuteAsync(sql, entity);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public async Task<int> DeleteAllAsync()
        {
            var result = _connection.Count<T>();
            await _connection.ExecuteAsync($"TRUNCATE TABLE {_tableName}");
            return result;
        }

        #endregion Delete
    }
}
